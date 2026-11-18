using System;
using System.Threading;

namespace IndependentWork14_1
{
    public class RetryPolicy
    {
        private int _maxRetries;
        private TimeSpan _initialDelay;
        private double _jitterFactor;
        private Random _random = new Random();

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
        {
            for (int retryAttempt = 0; retryAttempt <= _maxRetries; retryAttempt++)
            {
                Console.WriteLine("  Attempt " + (retryAttempt + 1) + " of " + (_maxRetries + 1));

                try
                {
                    T result = operation();
                    Console.WriteLine("  Success");
                    return result;
                }
                catch (Exception ex)
                {
                    bool canRetry = shouldRetry == null || shouldRetry(ex);

                    if (!canRetry)
                    {
                        Console.WriteLine("  Failed: " + ex.Message + ". This error is not retried");
                        throw;
                    }

                    if (retryAttempt == _maxRetries)
                    {
                        Console.WriteLine("  Failed: " + ex.Message + ". No retries left");
                        throw;
                    }

                    double delayMs = _initialDelay.TotalMilliseconds
                        * Math.Pow(2, retryAttempt)
                        * (1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor);

                    Console.WriteLine("  Failed: " + ex.Message + ". Waiting " + Math.Round(delayMs) + " ms before retry " + (retryAttempt + 1));
                    Thread.Sleep(TimeSpan.FromMilliseconds(delayMs));
                }
            }

            throw new InvalidOperationException("Retry policy finished without result");
        }
    }

    class Program
    {
        static int serverCalls = 0;

        static string LoadDataFromServer()
        {
            serverCalls++;

            if (serverCalls <= 2)
            {
                throw new InvalidOperationException("Server is busy");
            }

            return "Data loaded";
        }

        static string AlwaysFails()
        {
            throw new TimeoutException("Server is not responding");
        }

        static int ParseNumber(string text)
        {
            return int.Parse(text);
        }

        static void Main(string[] args)
        {
            RetryPolicy policy = new RetryPolicy(3, TimeSpan.FromMilliseconds(200), 0.2);

            Console.WriteLine("Scenario 1: fails twice, then succeeds");
            string data = policy.Execute<string>(() => LoadDataFromServer());
            Console.WriteLine("Result: " + data);
            Console.WriteLine();

            Console.WriteLine("Scenario 2: always fails");
            try
            {
                policy.Execute<string>(() => AlwaysFails());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Final failure: " + ex.Message);
            }
            Console.WriteLine();

            Console.WriteLine("Scenario 3: shouldRetry, only TimeoutException is retried");
            try
            {
                int number = policy.Execute<int>(() => ParseNumber("abc"), ex => ex is TimeoutException);
                Console.WriteLine("Result: " + number);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Final failure: " + ex.Message);
            }
        }
    }
}