using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    class Program
    {
        static bool IsPrime(int n)
        {
            if (n < 2)
            {
                return false;
            }

            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        static List<int> CreateNumbers(int count)
        {
            Random random = new Random(42);
            List<int> numbers = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                numbers.Add(random.Next(1, 1000001));
            }

            return numbers;
        }

        static void CompareLinqAndPlinq(int count)
        {
            List<int> numbers = CreateNumbers(count);

            Stopwatch sw = Stopwatch.StartNew();
            List<double> linqResult = numbers.Where(n => IsPrime(n)).Select(n => Math.Sqrt(n)).ToList();
            sw.Stop();
            double linqMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2);

            sw.Restart();
            List<double> plinqResult = numbers.AsParallel().Where(n => IsPrime(n)).Select(n => Math.Sqrt(n)).ToList();
            sw.Stop();
            double plinqMs = Math.Round(sw.Elapsed.TotalMilliseconds, 2);

            Console.WriteLine("Elements: " + count + " | LINQ: " + linqMs + " ms | PLINQ: " + plinqMs + " ms");
            Console.WriteLine("Found: LINQ " + linqResult.Count + ", PLINQ " + plinqResult.Count);
        }

        static void SideEffectsDemo()
        {
            int count = 1000000;
            Console.WriteLine("Expected value: " + count);

            for (int run = 1; run <= 3; run++)
            {
                int counter = 0;
                Enumerable.Range(0, count).AsParallel().ForAll(n => { counter++; });
                Console.WriteLine("Run " + run + ", no synchronization: " + counter);
            }

            int lockCounter = 0;
            object locker = new object();
            Enumerable.Range(0, count).AsParallel().ForAll(n =>
            {
                lock (locker)
                {
                    lockCounter++;
                }
            });
            Console.WriteLine("With lock: " + lockCounter);

            int interlockedCounter = 0;
            Enumerable.Range(0, count).AsParallel().ForAll(n => Interlocked.Increment(ref interlockedCounter));
            Console.WriteLine("With Interlocked: " + interlockedCounter);

            ConcurrentBag<int> bag = new ConcurrentBag<int>();
            Enumerable.Range(0, count).AsParallel().ForAll(n => bag.Add(n));
            Console.WriteLine("With ConcurrentBag: " + bag.Count);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Processor cores: " + Environment.ProcessorCount);
            Console.WriteLine();

            Console.WriteLine("Warm-up:");
            CompareLinqAndPlinq(1000);
            Console.WriteLine();

            Console.WriteLine("Performance tests (operation: prime check, then square root):");
            CompareLinqAndPlinq(1000);
            CompareLinqAndPlinq(1000000);
            CompareLinqAndPlinq(10000000);
            Console.WriteLine();

            Console.WriteLine("Side effects in PLINQ:");
            SideEffectsDemo();
        }
    }
}