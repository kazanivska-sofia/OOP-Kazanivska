using System;
using System.Collections.Generic;

namespace Lab9SelfV11
{
    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;

        public override string ToString() => $"{Name} ({Category}) — {Price} грн";
    }

    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            T maxItem = default!;
            TKey maxKey = default!;
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }

            if (first)
            {
                throw new InvalidOperationException("Колекція порожня.");
            }

            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== САМОСТІЙНА РОБОТА №10: Делегати й лямбда-вирази (CollectionUtils) ===\n");

            Console.WriteLine("--- 1. Робота з колекцією чисел (List<int>) ---");
            var numbers = new List<int> { 4, 15, 8, 23, 42, 16, 7 };

            var evenNumbers = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            var squaredNumbers = CollectionUtils.Map(evenNumbers, n => n * n);
            int maxNumber = CollectionUtils.MaxBy(numbers, n => n);

            Console.Write("Парні числа у квадраті: ");
            CollectionUtils.ForEach(squaredNumbers, n => Console.Write($"{n} "));
            Console.WriteLine($"\nМаксимальне число: {maxNumber}\n");

            Console.WriteLine("--- 2. Робота з колекцією рядків (List<string>) ---");
            var languages = new List<string> { "C#", "Python", "JavaScript", "Go", "TypeScript" };

            var longLanguages = CollectionUtils.Filter(languages, lang => lang.Length >= 6);
            var upperLanguages = CollectionUtils.Map(longLanguages, lang => $"{lang.ToUpper()} ({lang.Length} симв.)");
            string longestLanguage = CollectionUtils.MaxBy(languages, lang => lang.Length);

            Console.WriteLine("Мови програмування (>= 6 символів):");
            CollectionUtils.ForEach(upperLanguages, info => Console.WriteLine($"  * {info}"));
            Console.WriteLine($"Найдовша назва: {longestLanguage}\n");

            Console.WriteLine("--- 3. Робота з колекцією об'єктів (List<Product>) ---");
            var products = new List<Product>
            {
                new Product { Name = "Ноутбук ASUS", Category = "Комп'ютери", Price = 32000m },
                new Product { Name = "Мишка Logitech", Category = "Аксесуари", Price = 950m },
                new Product { Name = "Монітор Dell", Category = "Монітори", Price = 11500m },
                new Product { Name = "Клавіатура Keychron", Category = "Аксесуари", Price = 3400m }
            };

            var expensiveProducts = CollectionUtils.Filter(products, p => p.Price > 2000m);
            var productLabels = CollectionUtils.Map(expensiveProducts, p => $"{p.Name} — {p.Price} грн");
            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);

            Console.WriteLine("Товари дорожчі за 2000 грн:");
            CollectionUtils.ForEach(productLabels, label => Console.WriteLine($"  - {label}"));
            Console.WriteLine($"Найдорожчий товар: {mostExpensive}");
        }
    }
}