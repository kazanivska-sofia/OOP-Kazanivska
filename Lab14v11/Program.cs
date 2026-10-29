using System;
using System.Collections.Generic;

namespace Lab14v11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №14: HashSet, Queue, Stack ===");
            Console.WriteLine("=== Варіант 11: Система кешування ===\n");

            // 1. HashSet: Унікальні ключі кешу
            HashSet<string> cacheKeys = new HashSet<string>();

            // 2. Queue: Черга для видалення елементів кешу (LRU)
            Queue<string> cacheQueue = new Queue<string>();

            // 3. Stack: Історія звернень (останні переглянуті ключі)
            Stack<string> accessHistory = new Stack<string>();

            // --- ДЕМОНСТРАЦІЯ HASHSET (Унікальність) ---
            Console.WriteLine("--- 1. РОБОТА З HASHSET (Унікальні ключі кешу) ---");
            AddCacheKey(cacheKeys, cacheQueue, accessHistory, "user_101");
            AddCacheKey(cacheKeys, cacheQueue, accessHistory, "settings_theme");
            AddCacheKey(cacheKeys, cacheQueue, accessHistory, "user_101"); // Дублікат
            AddCacheKey(cacheKeys, cacheQueue, accessHistory, "product_list");

            Console.WriteLine($"\nЗагальна кількість унікальних ключів у кеші: {cacheKeys.Count}");
            Console.WriteLine($"Чи міститься 'settings_theme' у кеші? {cacheKeys.Contains("settings_theme")}");

            // --- ДЕМОНСТРАЦІЯ QUEUE (FIFO) ---
            Console.WriteLine("\n--- 2. РОБОТА З QUEUE (Черга на видалення кешу - FIFO) ---");
            Console.WriteLine($"Найстаріший елемент у черзі (Peek): {cacheQueue.Peek()}");
            
            Console.WriteLine("Очищення найстаріших елементів кешу:");
            while (cacheQueue.Count > 0)
            {
                string removedKey = cacheQueue.Dequeue();
                cacheKeys.Remove(removedKey);
                Console.WriteLine($"Видалено з черги та кешу: {removedKey}");
            }

            // --- ДЕМОНСТРАЦІЯ STACK (LIFO) ---
            Console.WriteLine("\n--- 3. РОБОТА З STACK (Історія звернень - LIFO) ---");
            Console.WriteLine($"Останнє звернення (Peek): {accessHistory.Peek()}");

            Console.WriteLine("Перегляд історії звернень (у зворотному порядку):");
            while (accessHistory.Count > 0)
            {
                string lastAccessed = accessHistory.Pop();
                Console.WriteLine($"Звернення до: {lastAccessed}");
            }
        }

        static void AddCacheKey(HashSet<string> keys, Queue<string> queue, Stack<string> history, string key)
        {
            if (keys.Add(key))
            {
                queue.Enqueue(key);
                Console.WriteLine($"[Кеш] Успішно додано новий ключ: '{key}'");
            }
            else
            {
                Console.WriteLine($"[Кеш] Спроба додати дублікат: '{key}' (ігнорується)");
            }

            // Записуємо звернення в історію
            history.Push(key);
        }
    }
}