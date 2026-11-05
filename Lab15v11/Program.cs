using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lab15v11
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
    }

    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // --- ТЕСТОВІ ДАНІ (10-20 елементів) ---
            var venues = new List<Venue>
            {
                new Venue { Id = 1, Name = "Олімпійський Арена", Capacity = 1000 },
                new Venue { Id = 2, Name = "Міський Палац Культури", Capacity = 400 },
                new Venue { Id = 3, Name = "Tech Hub Hall", Capacity = 600 },
                new Venue { Id = 4, Name = "Камерний Зал", Capacity = 150 }
            };

            var events = new List<Event>
            {
                new Event { Id = 1, Name = "IT Conference 2024", Date = new DateTime(2024, 05, 15), LocationId = 1 },
                new Event { Id = 2, Name = "Rock Concert", Date = new DateTime(2024, 08, 20), LocationId = 1 },
                new Event { Id = 3, Name = "Startup Pitch Day", Date = new DateTime(2024, 11, 10), LocationId = 3 },
                new Event { Id = 4, Name = "Art Exhibition", Date = new DateTime(2025, 01, 12), LocationId = 2 },
                new Event { Id = 5, Name = "Chamber Music Night", Date = new DateTime(2024, 03, 05), LocationId = 4 },
                new Event { Id = 6, Name = "AI Summit 2024", Date = new DateTime(2024, 09, 30), LocationId = 3 },
                new Event { Id = 7, Name = "CyberSecurity Workshop", Date = new DateTime(2024, 06, 18), LocationId = 3 },
                new Event { Id = 8, Name = "Gamedev Expo", Date = new DateTime(2024, 12, 01), LocationId = 1 },
                new Event { Id = 9, Name = "Charity Marathon Gala", Date = new DateTime(2024, 04, 25), LocationId = 2 },
                new Event { Id = 10, Name = "Design Forum", Date = new DateTime(2025, 02, 14), LocationId = 3 }
            };

            Stopwatch sw = new Stopwatch();

            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №15: Фільтри, Join, Агрегації та Продуктивність ===\n");

            // ----------------------------------------------------------------------------------
            // ЗАПИТ 1: Знайти всі події 2024 року на локаціях з місткістю > 500
            // ----------------------------------------------------------------------------------
            Console.WriteLine("--- 1. Події 2024 року на локаціях з місткістю > 500 ---");

            // Method Syntax
            sw.Restart();
            var q1Method = events
                .Where(e => e.Date.Year == 2024)
                .Join(venues.Where(v => v.Capacity > 500),
                      e => e.LocationId,
                      v => v.Id,
                      (e, v) => new { EventName = e.Name, Date = e.Date, VenueName = v.Name, Capacity = v.Capacity })
                .ToList();
            sw.Stop();
            long timeQ1Method = sw.ElapsedTicks;

            // Query Syntax
            sw.Restart();
            var q1Query = (from e in events
                          join v in venues on e.LocationId equals v.Id
                          where e.Date.Year == 2024 && v.Capacity > 500
                          select new { EventName = e.Name, Date = e.Date, VenueName = v.Name, Capacity = v.Capacity }).ToList();
            sw.Stop();
            long timeQ1Query = sw.ElapsedTicks;

            foreach (var item in q1Method)
            {
                Console.WriteLine($"  - Подія: {item.EventName} ({item.Date:yyyy-MM-dd}) | Локація: {item.VenueName} (Місткість: {item.Capacity})");
            }
            Console.WriteLine($"Час виконання (Method Syntax): {timeQ1Method} ticks | (Query Syntax): {timeQ1Query} ticks\n");

            // ----------------------------------------------------------------------------------
            // ЗАПИТ 2: Об'єднати події з їхніми локаціями (Join)
            // ----------------------------------------------------------------------------------
            Console.WriteLine("--- 2. Об'єднання подій з локаціями (Join) ---");

            sw.Restart();
            var q2Method = events
                .Join(venues,
                      e => e.LocationId,
                      v => v.Id,
                      (e, v) => new { EventName = e.Name, VenueName = v.Name })
                .ToList();
            sw.Stop();
            long timeQ2Method = sw.ElapsedTicks;

            sw.Restart();
            var q2Query = (from e in events
                          join v in venues on e.LocationId equals v.Id
                          select new { EventName = e.Name, VenueName = v.Name }).ToList();
            sw.Stop();
            long timeQ2Query = sw.ElapsedTicks;

            foreach (var item in q2Method.Take(5)) // Виведемо перші 5 для лаконічності
            {
                Console.WriteLine($"  - Подія: '{item.EventName}' -> Локація: '{item.VenueName}'");
            }
            Console.WriteLine($"Час виконання (Method Syntax): {timeQ2Method} ticks | (Query Syntax): {timeQ2Query} ticks\n");

            // ----------------------------------------------------------------------------------
            // ЗАПИТ 3: Підрахувати кількість подій для кожної локації
            // ----------------------------------------------------------------------------------
            Console.WriteLine("--- 3. Кількість подій для кожної локації ---");

            sw.Restart();
            var q3Method = venues
                .GroupJoin(events,
                           v => v.Id,
                           e => e.LocationId,
                           (v, eventGroup) => new { VenueName = v.Name, Count = eventGroup.Count() })
                .ToList();
            sw.Stop();
            long timeQ3Method = sw.ElapsedTicks;

            sw.Restart();
            var q3Query = (from v in venues
                          join e in events on v.Id equals e.LocationId into eventGroup
                          select new { VenueName = v.Name, Count = eventGroup.Count() }).ToList();
            sw.Stop();
            long timeQ3Query = sw.ElapsedTicks;

            foreach (var item in q3Method)
            {
                Console.WriteLine($"  - Локація: '{item.VenueName}' | Кількість подій: {item.Count}");
            }
            Console.WriteLine($"Час виконання (Method Syntax): {timeQ3Method} ticks | (Query Syntax): {timeQ3Query} ticks\n");

            // ----------------------------------------------------------------------------------
            // ЗАПИТ 4: Знайти подію з найближчою датою
            // ----------------------------------------------------------------------------------
            Console.WriteLine("--- 4. Подія з найближчою датою ---");

            sw.Restart();
            var q4Method = events
                .OrderBy(e => e.Date)
                .FirstOrDefault();
            sw.Stop();
            long timeQ4Method = sw.ElapsedTicks;

            sw.Restart();
            var q4Query = (from e in events
                          orderby e.Date ascending
                          select e).FirstOrDefault();
            sw.Stop();
            long timeQ4Query = sw.ElapsedTicks;

            if (q4Method != null)
            {
                Console.WriteLine($"  - Найраніша/найближча подія: '{q4Method.Name}' (Дата: {q4Method.Date:yyyy-MM-dd})");
            }
            Console.WriteLine($"Час виконання (Method Syntax): {timeQ4Method} ticks | (Query Syntax): {timeQ4Query} ticks\n");
        }
    }
}