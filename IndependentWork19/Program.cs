using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork19
{
    public class InsufficientFundsException : Exception
    {
        public decimal RequestedAmount { get; }
        public decimal CurrentBalance { get; }

        public InsufficientFundsException() { }

        public InsufficientFundsException(string message) : base(message) { }

        public InsufficientFundsException(string message, Exception inner) : base(message, inner) { }

        public InsufficientFundsException(string message, decimal requested, decimal balance) : base(message)
        {
            RequestedAmount = requested;
            CurrentBalance = balance;
        }
    }

    class Program
    {
        static int apiCalls = 0;
        static int dbCalls = 0;
        static bool databaseDown = true;
        static int queueAttempt = 0;

        static void Withdraw(decimal balance, decimal amount)
        {
            if (amount > balance)
            {
                throw new InsufficientFundsException("Not enough funds on the account", amount, balance);
            }

            Console.WriteLine("Withdrawn " + amount);
        }

        static void ExceptionDemo()
        {
            decimal balance = 500;

            try
            {
                Withdraw(balance, 200);
                Withdraw(balance, 800);
            }
            catch (InsufficientFundsException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Requested: " + ex.RequestedAmount + ", balance: " + ex.CurrentBalance);
            }
            finally
            {
                Console.WriteLine("Operation finished");
            }
        }

        static string CallWeatherApi()
        {
            apiCalls++;
            Console.WriteLine("  API call #" + apiCalls);

            if (apiCalls <= 2)
            {
                throw new HttpRequestException("Service temporarily unavailable (503)");
            }

            return "Weather in Rivne: +5 C";
        }

        static void RetryScenario()
        {
            ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(200),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine("  Retry #" + (args.AttemptNumber + 1) + " in " + Math.Round(args.RetryDelay.TotalMilliseconds) + " ms, reason: " + args.Outcome.Exception?.Message);
                        return default;
                    }
                })
                .Build();

            try
            {
                string result = pipeline.Execute(() => CallWeatherApi());
                Console.WriteLine("Result: " + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Final failure: " + ex.Message);
            }
        }

        static string QueryDatabase()
        {
            dbCalls++;

            if (databaseDown)
            {
                throw new TimeoutException("Database connection timed out");
            }

            return "10 rows";
        }

        static void CircuitBreakerScenario()
        {
            ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<TimeoutException>(),
                    FailureRatio = 0.5,
                    MinimumThroughput = 3,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    BreakDuration = TimeSpan.FromSeconds(2),
                    OnOpened = args =>
                    {
                        Console.WriteLine("  [circuit] OPEN: requests are rejected");
                        return default;
                    },
                    OnHalfOpened = args =>
                    {
                        Console.WriteLine("  [circuit] HALF-OPEN: testing the database");
                        return default;
                    },
                    OnClosed = args =>
                    {
                        Console.WriteLine("  [circuit] CLOSED: database works again");
                        return default;
                    }
                })
                .Build();

            for (int i = 1; i <= 8; i++)
            {
                if (i == 7)
                {
                    databaseDown = false;
                    Console.WriteLine("  Database recovered, waiting 2.5 s");
                    Thread.Sleep(2500);
                }

                try
                {
                    string result = pipeline.Execute(() => QueryDatabase());
                    Console.WriteLine("Request " + i + ": OK, " + result);
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine("Request " + i + ": rejected, circuit is open (database not called)");
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine("Request " + i + ": failed, " + ex.Message);
                }
            }

            Console.WriteLine("Database was really called " + dbCalls + " times out of 8 requests");
        }

        static async Task SendToQueue(CancellationToken token)
        {
            queueAttempt++;
            int responseTime = queueAttempt <= 2 ? 1000 : 100;

            Console.WriteLine("  Attempt " + queueAttempt + ": sending, queue response time " + responseTime + " ms");
            await Task.Delay(responseTime, token);
            Console.WriteLine("  Attempt " + queueAttempt + ": message sent");
        }

        static async Task RetryTimeoutScenario()
        {
            ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(100),
                    BackoffType = DelayBackoffType.Constant,
                    OnRetry = args =>
                    {
                        Console.WriteLine("  Timeout. Retry #" + (args.AttemptNumber + 1) + " in " + Math.Round(args.RetryDelay.TotalMilliseconds) + " ms");
                        return default;
                    }
                })
                .AddTimeout(TimeSpan.FromMilliseconds(300))
                .Build();

            try
            {
                await pipeline.ExecuteAsync(async token =>
                {
                    await SendToQueue(token);
                });
                Console.WriteLine("Result: message delivered");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Final failure: " + ex.Message);
            }
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Custom exception:");
            ExceptionDemo();
            Console.WriteLine();

            Console.WriteLine("Scenario 1: Retry with exponential backoff (external API)");
            RetryScenario();
            Console.WriteLine();

            Console.WriteLine("Scenario 2: Circuit Breaker (database)");
            CircuitBreakerScenario();
            Console.WriteLine();

            Console.WriteLine("Scenario 3: Retry + Timeout (message queue)");
            await RetryTimeoutScenario();
        }
    }
}