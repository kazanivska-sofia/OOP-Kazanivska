using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace IndependentWork14
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;
        private readonly Random _random = new Random();

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            if (maxRetries < 0)
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Кількість повторних спроб не може бути від'ємною.");
            if (initialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(initialDelay), "Початкова затримка не може бути від'ємною.");
            if (jitterFactor < 0 || jitterFactor > 1)
                throw new ArgumentOutOfRangeException(nameof(jitterFactor), "Коефіцієнт jitter має бути в межах [0, 1].");

            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    bool canRetry = retryAttempt < _maxRetries && (shouldRetry == null || shouldRetry(ex));

                    if (!canRetry)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [Помилка] Повторні спроби припинено ({ex.GetType().Name}: {ex.Message}).");
                        throw;
                    }

                    double multiplier = Math.Pow(2, retryAttempt) * (1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor);
                    TimeSpan delay = TimeSpan.FromMilliseconds(_initialDelay.TotalMilliseconds * Math.Max(0, multiplier));

                    retryAttempt++;
                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss.fff}] Спроба #{retryAttempt} не вдалася ({ex.GetType().Name}: {ex.Message}). " +
                        $"Очікування {delay.TotalMilliseconds:F0} мс перед наступною спробою...");

                    Thread.Sleep(delay);
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== САМОСТІЙНА РОБОТА №15: Патерн Retry з Exponential Backoff та Jitter ===\n");

            var retryPolicy = new RetryPolicy(
                maxRetries: 3,
                initialDelay: TimeSpan.FromMilliseconds(300),
                jitterFactor: 0.25);

            Console.WriteLine("--- Сценарій 1: Успішне виконання після тимчасових збоїв ---");
            int attemptCounter1 = 0;
            try
            {
                string result = retryPolicy.Execute(
                    operation: () =>
                    {
                        attemptCounter1++;
                        if (attemptCounter1 <= 2)
                        {
                            throw new HttpRequestException($"Тимчасова помилка шлюзу (503 Service Unavailable), виклик #{attemptCounter1}");
                        }
                        return "Платіж успішно підтверджено (ID: TXN-9042)";
                    },
                    shouldRetry: ex => ex is HttpRequestException || ex is TimeoutException
                );

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] РЕЗУЛЬТАТ: {result}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неочікуваний збій: {ex.Message}\n");
            }

            Console.WriteLine("--- Сценарій 2: Остаточна невдача після вичерпання всіх спроб ---");
            int attemptCounter2 = 0;
            try
            {
                int data = retryPolicy.Execute<int>(
                    operation: () =>
                    {
                        attemptCounter2++;
                        throw new IOException($"Віддалений сервер не відповідає (виклик #{attemptCounter2})");
                    },
                    shouldRetry: ex => ex is IOException
                );
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ПЕРЕХОПЛЕНО ФІНАЛЬНИЙ ВИНЯТОК у Main: {ex.Message}\n");
            }

            Console.WriteLine("--- Сценарій 3: Миттєва зупинка для невиправного винятку (shouldRetry = false) ---");
            try
            {
                retryPolicy.Execute<string>(
                    operation: () => throw new ArgumentException("Некоректний формат номера картки (повтор безглуздий)"),
                    shouldRetry: ex => ex is HttpRequestException // ArgumentException не підлягає повтору
                );
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ПЕРЕХОПЛЕНО КРИТИЧНИЙ ВИНЯТОК без зайвих повторів: {ex.Message}");
            }
        }
    }
}