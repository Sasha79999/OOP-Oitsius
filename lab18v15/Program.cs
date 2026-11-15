using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Lab18V15
{
    public class FileProcessor
    {
        private int _callCount = 0;

        public string LoadConfiguration(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Шлях не може бути порожнім");
            }

            _callCount++;

            if (_callCount <= 2)
            {
                throw new IOException("Файл тимчасово недоступний");
            }

            return "Конфігурація з файлу " + path;
        }
    }

    public class NetworkClient
    {
        private int _callCount = 0;

        public string DownloadConfiguration(string url)
        {
            _callCount++;

            if (_callCount <= 3)
            {
                throw new HttpRequestException("Сервер не відповідає");
            }

            return "Конфігурація з " + url;
        }
    }

    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(Func<T> operation, int retryCount = 3, TimeSpan initialDelay = default, Func<Exception, bool>? shouldRetry = null)
        {
            if (initialDelay == TimeSpan.Zero)
            {
                initialDelay = TimeSpan.FromMilliseconds(100);
            }

            int attempt = 0;

            while (true)
            {
                Console.WriteLine("Спроба " + (attempt + 1) + "...");

                try
                {
                    T result = operation();
                    Console.WriteLine("Спроба " + (attempt + 1) + " успішна");
                    return result;
                }
                catch (Exception ex)
                {
                    bool retryAllowed = shouldRetry == null || shouldRetry(ex);

                    if (!retryAllowed)
                    {
                        Console.WriteLine("Спроба " + (attempt + 1) + " не вдалася: " + ex.Message + ". Для цього винятку повтор не виконується");
                        throw;
                    }

                    if (attempt >= retryCount)
                    {
                        Console.WriteLine("Спроба " + (attempt + 1) + " не вдалася: " + ex.Message + ". Спроби вичерпано");
                        throw;
                    }

                    double delayMs = initialDelay.TotalMilliseconds * Math.Pow(2, attempt);
                    Console.WriteLine("Спроба " + (attempt + 1) + " не вдалася: " + ex.Message + ". Повтор через " + delayMs + " мс");
                    Thread.Sleep(TimeSpan.FromMilliseconds(delayMs));
                    attempt++;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Func<Exception, bool> shouldRetry = exception => exception is IOException || exception is HttpRequestException;

            Console.WriteLine("FileProcessor: LoadConfiguration (IOException перші 2 рази)");
            FileProcessor fileProcessor = new FileProcessor();
            string fileConfig = RetryHelper.ExecuteWithRetry(() => fileProcessor.LoadConfiguration("config.json"), 3, TimeSpan.FromMilliseconds(100), shouldRetry);
            Console.WriteLine("Результат: " + fileConfig);

            Console.WriteLine();
            Console.WriteLine("NetworkClient: DownloadConfiguration (HttpRequestException перші 3 рази)");
            NetworkClient networkClient = new NetworkClient();
            string networkConfig = RetryHelper.ExecuteWithRetry(() => networkClient.DownloadConfiguration("https://example.com/config"), 3, TimeSpan.FromMilliseconds(100), shouldRetry);
            Console.WriteLine("Результат: " + networkConfig);

            Console.WriteLine();
            Console.WriteLine("shouldRetry: порожній шлях дає ArgumentException, повтор не виконується");
            try
            {
                RetryHelper.ExecuteWithRetry(() => fileProcessor.LoadConfiguration(""), 3, TimeSpan.FromMilliseconds(100), shouldRetry);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Виняток дійшов до Main: " + ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Ліміт спроб: retryCount = 2, а помилок буде 3");
            NetworkClient limitedClient = new NetworkClient();
            try
            {
                RetryHelper.ExecuteWithRetry(() => limitedClient.DownloadConfiguration("https://example.com/config"), 2, TimeSpan.FromMilliseconds(100), shouldRetry);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Спроби вичерпано, виняток дійшов до Main: " + ex.Message);
            }
        }
    }
}