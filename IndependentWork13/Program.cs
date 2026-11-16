using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork13
{
    internal class Program
    {
        private static int _dbAttempts = 0;
        private static int _brokerAttempts = 0;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== САМОСТІЙНА РОБОТА №14: Кейси Polly / ResiliencePipeline ===\n");

            await RunDatabaseRetryScenarioAsync();
            await RunMessageBrokerCircuitBreakerScenarioAsync();
            await RunAnalyticsTimeoutFallbackScenarioAsync();
        }

        static async Task RunDatabaseRetryScenarioAsync()
        {
            Console.WriteLine("--- Сценарій 1: Доступ до бази даних (Retry з експоненційною затримкою) ---");

            var dbPipeline = new ResiliencePipelineBuilder<string>()
                .AddRetry(new RetryStrategyOptions<string>
                {
                    ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(500),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [DB Retry #{args.AttemptNumber + 1}] Повтор через {args.RetryDelay.TotalMilliseconds:F0} мс. Причина: {args.Outcome.Exception?.Message}");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            try
            {
                string data = await dbPipeline.ExecuteAsync(async ct =>
                {
                    await Task.Delay(50, ct);
                    return QueryDatabase("SELECT * FROM Orders WHERE Status = 'Active'");
                });
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Успішний результат БД: {data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Остаточна помилка БД: {ex.Message}");
            }

            Console.WriteLine("--- Кінець Сценарію 1 ---\n");
        }

        static string QueryDatabase(string sql)
        {
            _dbAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба #{_dbAttempts}: виконання запиту до БД...");

            if (_dbAttempts <= 2)
            {
                throw new TimeoutException("Connection pool timeout expired (БД тимчасово перевантажена).");
            }

            return "Отримано 42 активних замовлення";
        }

        static async Task RunMessageBrokerCircuitBreakerScenarioAsync()
        {
            Console.WriteLine("--- Сценарій 2: Відправка повідомлень у чергу (Circuit Breaker) ---");

            var brokerPipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 3,
                    BreakDuration = TimeSpan.FromSeconds(2),
                    OnOpened = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Circuit OPENED] Ланцюг розімкнено на {args.BreakDuration.TotalSeconds} с через серію збоїв брокера!");
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Circuit CLOSED] Роботу брокера відновлено.");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Circuit HALF-OPEN] Пробна перевірка брокера...");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            for (int msgId = 1; msgId <= 6; msgId++)
            {
                try
                {
                    await brokerPipeline.ExecuteAsync(async ct =>
                    {
                        await Task.Delay(30, ct);
                        PublishToQueue($"Event_OrderCreated_{msgId}");
                    });
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Повідомлення #{msgId} успішно доставлено у чергу.");
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Повідомлення #{msgId} миттєво відхилено (Circuit Breaker відкритий — не навантажуємо брокер).");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Помилка відправки #{msgId}: {ex.Message}");
                }

                if (msgId == 4)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Очікування 2.2 с для переходу Circuit Breaker у стан Half-Open...");
                    await Task.Delay(2200);
                }
            }

            Console.WriteLine("--- Кінець Сценарію 2 ---\n");
        }

        static void PublishToQueue(string message)
        {
            _brokerAttempts++;
            if (_brokerAttempts <= 3)
            {
                throw new IOException("RabbitMQ broker connection reset.");
            }
        }

        static async Task RunAnalyticsTimeoutFallbackScenarioAsync()
        {
            Console.WriteLine("--- Сценарій 3: Запит до сервісу аналітики (Timeout + Fallback) ---");

            var analyticsPipeline = new ResiliencePipelineBuilder<string>()
                .AddFallback(new FallbackStrategyOptions<string>
                {
                    ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutRejectedException>(),
                    FallbackAction = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Fallback] Спрацював резервний сценарій через перевищення часу очікування!");
                        return Outcome.FromResultAsValueTask("Кешований звіт за вчорашній день (15 400 грн)");
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromMilliseconds(400),
                    OnTimeout = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Timeout] Операцію перервано після {args.Timeout.TotalMilliseconds} мс!");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            string report = await analyticsPipeline.ExecuteAsync(async ct =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Запит на побудову важкого звіту (триватиме ~1500 мс)...");
                await Task.Delay(1500, ct); 
                return "Свіжий звіт у реальному часі";
            });

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Отриманий звіт для користувача: {report}");
            Console.WriteLine("--- Кінець Сценарію 3 ---\n");
        }
    }
}