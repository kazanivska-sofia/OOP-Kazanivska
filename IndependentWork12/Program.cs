using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== САМОСТІЙНА РОБОТА №13: PLINQ — продуктивність та безпека ===");
            Console.WriteLine($"Кількість логічних ядер процесора: {Environment.ProcessorCount}\n");

            Console.WriteLine("--- 1. Порівняння продуктивності на різних обсягах даних (1M, 5M, 10M) ---");
            int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };

            foreach (int size in sizes)
            {
                Console.WriteLine($"\n[Розмір колекції: {size:N0} елементів]");
                var data = GenerateRandomList(size, seed: 42);

                CompareOperations(
                    data,
                    operationName: "Легка операція (x * 2)",
                    linqFunc: list => list.Where(x => x % 2 == 0).Select(x => x * 2L).Count(),
                    plinqFunc: list => list.AsParallel().Where(x => x % 2 == 0).Select(x => x * 2L).Count()
                );

                CompareOperations(
                    data,
                    operationName: "Важка операція (Math + PrimeCheck)",
                    linqFunc: list => list.Where(x => x > 100).Select(HeavyMathOperation).Count(),
                    plinqFunc: list => list.AsParallel().Where(x => x > 100).Select(HeavyMathOperation).Count()
                );
            }

            Console.WriteLine("\n--- 2. Дослідження безпеки та побічних ефектів у PLINQ ---");
            DemonstrateSideEffectsAndFix();
        }

        static List<int> GenerateRandomList(int count, int seed)
        {
            var random = new Random(seed);
            var list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(random.Next(1, 100_000));
            }
            return list;
        }

        static double HeavyMathOperation(int n)
        {
            double acc = Math.Sqrt(n) * Math.Sin(n) + Math.Cos(n);
            // Перевірка на простоту (штучне навантаження CPU)
            int limit = (int)Math.Sqrt(n);
            bool isPrime = n > 1;
            for (int i = 2; i <= limit; i++)
            {
                if (n % i == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            return isPrime ? acc * 1.5 : acc;
        }

        static void CompareOperations(
            List<int> data,
            string operationName,
            Func<List<int>, int> linqFunc,
            Func<List<int>, int> plinqFunc)
        {
            // Прогрів
            _ = linqFunc(data.Take(100).ToList());
            _ = plinqFunc(data.Take(100).ToList());

            var sw = Stopwatch.StartNew();
            int linqCount = linqFunc(data);
            sw.Stop();
            long linqMs = sw.ElapsedMilliseconds;

            sw.Restart();
            int plinqCount = plinqFunc(data);
            sw.Stop();
            long plinqMs = sw.ElapsedMilliseconds;

            double speedup = plinqMs > 0 ? (double)linqMs / plinqMs : 0;
            string verdict = plinqMs < linqMs
                ? $"PLINQ швидший у {speedup:F2}x"
                : $"LINQ швидший у {(linqMs > 0 ? (double)plinqMs / linqMs : 1):F2}x (накладні витрати потоків)";

            Console.WriteLine($"  * {operationName,-35} | LINQ: {linqMs,5} мс | PLINQ: {plinqMs,5} мс | {verdict} (рез: {linqCount}/{plinqCount})");
        }

        static void DemonstrateSideEffectsAndFix()
        {
            var numbers = Enumerable.Range(1, 500_000).ToList();
            int expectedCount = numbers.Count(x => x % 2 == 0);
            Console.WriteLine($"Очікувана кількість парних чисел із 500 000: {expectedCount}");

            Console.WriteLine("\n[А] Небезпечний PLINQ (побічні ефекти без lock):");
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                int unsafeCounter = 0;
                var unsafeList = new List<int>();

                try
                {
                    numbers.AsParallel().ForAll(x =>
                    {
                        if (x % 2 == 0)
                        {
                            unsafeCounter++; 
                            if (unsafeList.Count < 1000)
                            {
                                unsafeList.Add(x); 
                            }
                        }
                    });
                    Console.WriteLine($"  Спроба {attempt}: unsafeCounter = {unsafeCounter} (втрачено {expectedCount - unsafeCounter} інкрементів!), unsafeList.Count = {unsafeList.Count}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Спроба {attempt}: Виняток при паралельному записі у List<T>: {ex.GetType().Name}");
                }
            }

            Console.WriteLine("\n[Б] Виправлений потокобезпечний PLINQ (Interlocked + ConcurrentBag<T>):");
            int safeCounter = 0;
            var safeBag = new ConcurrentBag<int>();

            numbers.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    Interlocked.Increment(ref safeCounter);
                    if (x <= 2000)
                    {
                        safeBag.Add(x);
                    }
                }
            });

            int purePlinqCount = numbers.AsParallel().Count(x => x % 2 == 0);

            Console.WriteLine($"  Interlocked.Increment результат: {safeCounter} (100% точність!)");
            Console.WriteLine($"  ConcurrentBag<T> елементів:    {safeBag.Count} (очікувано 1000)");
            Console.WriteLine($"  Чистий PLINQ .Count(...) без побічних ефектів: {purePlinqCount}");
        }
    }
}