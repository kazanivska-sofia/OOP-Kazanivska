using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Lab17v11
{
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            if (initialDelay == default)
            {
                initialDelay = TimeSpan.FromMilliseconds(500);
            }

            for (int attempt = 0; attempt <= retryCount; attempt++)
            {
                try
                {
                    if (attempt > 0)
                    {
                        Console.WriteLine($"  [Retry] Спроба #{attempt}...");
                    }
                    return operation();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [Помилка] Спроба #{attempt + 1} не вдалася ({ex.GetType().Name}): {ex.Message}");

                    bool canRetry = shouldRetry == null || shouldRetry(ex);
                    if (attempt == retryCount || !canRetry)
                    {
                        if (!canRetry)
                        {
                            Console.WriteLine("  [Стоп] Цей тип винятку не підлягає повторній спробі згідно з політикою shouldRetry.");
                        }
                        throw;
                    }

                    TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempt));
                    Console.WriteLine($"  [Очікування] Експоненційна затримка {delay.TotalSeconds:F2} сек перед наступною спробою...");
                    Thread.Sleep(delay);
                }
            }

            throw new InvalidOperationException("Непередбачене завершення циклу повторних спроб.");
        }
    }

    public class FileProcessor
    {
        private int _attemptCounter = 0;

        public List<string> LoadProductNames(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Шлях до файлу не може бути порожнім.");
            }

            _attemptCounter++;
            if (_attemptCounter <= 2)
            {
                throw new FileNotFoundException($"Файл '{path}' тимчасово заблокований або не знайдений.", path);
            }

            return new List<string> { "Ноутбук ASUS", "Монітор LG", "Клавіатура Keychron" };
        }
    }

    public class NetworkClient
    {
        private int _attemptCounter = 0;

        public List<string> GetProductsFromApi(string url)
        {
            _attemptCounter++;
            if (_attemptCounter <= 3)
            {
                throw new HttpRequestException($"Сервер {url} тимчасово недоступний (503 Service Unavailable).");
            }

            return new List<string> { "Смартфон Samsung", "Навушники Sony", "Планшет iPad" };
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Func<Exception, bool> retryPolicy = ex =>
                ex is FileNotFoundException || ex is HttpRequestException;

            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №17 (Варіант 11): Обробка I/O та мережевих помилок, Retry ===\n");

            Console.WriteLine("--- 1. Завантаження продуктів з файлу (FileProcessor) ---");
            var fileProcessor = new FileProcessor();
            try
            {
                var fileProducts = RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.LoadProductNames("products.csv"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: retryPolicy
                );

                Console.WriteLine($"Успішно завантажено з файлу: {string.Join(", ", fileProducts)}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критична помилка роботи з файлом: {ex.Message}\n");
            }

            Console.WriteLine("--- 2. Отримання продуктів з API (NetworkClient) ---");
            var networkClient = new NetworkClient();
            try
            {
                var apiProducts = RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.GetProductsFromApi("https://api.shop.ua/v1/products"),
                    retryCount: 4,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: retryPolicy
                );

                Console.WriteLine($"Успішно отримано з API: {string.Join(", ", apiProducts)}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критична помилка мережі: {ex.Message}\n");
            }

            Console.WriteLine("--- 3. Перевірка вибіркової обробки (shouldRetry) для нетимчасової помилки ---");
            try
            {
                RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.LoadProductNames(""), // Викличе ArgumentException
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: retryPolicy
                );
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Перехоплено очікуваний нетимчасовий виняток без повторів: {ex.Message}\n");
            }
        }
    }
}