using System;
using System.Collections.Generic;

namespace IndependentWork7
{
    public class Cache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly Queue<TKey> _keysOrder;

        public Cache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Ємність кешу повинна бути більшою за нуль.", nameof(capacity));
            }

            _capacity = capacity;
            _cache = new Dictionary<TKey, TValue>(capacity);
            _keysOrder = new Queue<TKey>(capacity);
        }

        public int Count => _cache.Count;

        public void Add(TKey key, TValue value)
        {
            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }

            if (_cache.Count >= _capacity)
            {
                TKey oldestKey = _keysOrder.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache.Add(key, value);
            _keysOrder.Enqueue(key);
        }

        public TValue Get(TKey key)
        {
            if (_cache.TryGetValue(key, out TValue? value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Ключ '{key}' не знайдено у кеші.");
        }

        public bool ContainsKey(TKey key)
        {
            return _cache.ContainsKey(key);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== САМОСТІЙНА РОБОТА №8: Узагальнений кеш Cache<TKey, TValue> (FIFO) ===\n");

            var cache = new Cache<string, string>(capacity: 3);

            Console.WriteLine("Додаємо 3 елементи (ємність = 3):");
            cache.Add("user_1", "Олена");
            cache.Add("user_2", "Іван");
            cache.Add("user_3", "Софія");
            Console.WriteLine($"Поточна кількість елементів (Count): {cache.Count}");
            Console.WriteLine($"Отримання за ключем 'user_1': {cache.Get("user_1")}\n");

            Console.WriteLine("Додаємо 4-й елемент ('user_4' -> 'Максим'). Найстаріший ('user_1') має витіснитися:");
            cache.Add("user_4", "Максим");

            Console.WriteLine($"Поточна кількість елементів (Count): {cache.Count}");
            Console.WriteLine($"Чи містить 'user_1' (ContainsKey): {cache.ContainsKey("user_1")}");
            Console.WriteLine($"Чи містить 'user_4' (ContainsKey): {cache.ContainsKey("user_4")}");

            try
            {
                Console.WriteLine("\nСпроба отримати витіснений ключ 'user_1':");
                cache.Get("user_1");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Очікуваний виняток: {ex.Message}");
            }
        }
    }
}