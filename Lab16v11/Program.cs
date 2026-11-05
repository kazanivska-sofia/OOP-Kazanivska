using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab16v11
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
    }

    public class Participant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }

    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Participant> Participants { get; set; } = new List<Participant>();
    }

    public static class EventExtensions
    {
        public static double TotalTicketRevenue(this IEnumerable<Event> events)
        {
            return events
                .SelectMany(e => e.Participants)
                .SelectMany(p => p.Tickets)
                .Sum(t => t.Price);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var events = new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Name = "Tech Conference 2026",
                    Participants = new List<Participant>
                    {
                        new Participant { Id = 1, Name = "Олена", Tickets = new List<Ticket> { new Ticket { Id = 101, Type = "VIP", Price = 1500 } } },
                        new Participant { Id = 2, Name = "Іван", Tickets = new List<Ticket> { new Ticket { Id = 102, Type = "Standard", Price = 800 } } }
                    }
                },
                new Event
                {
                    Id = 2,
                    Name = "DevFest Spring",
                    Participants = new List<Participant>
                    {
                        new Participant { Id = 3, Name = "Софія", Tickets = new List<Ticket> { new Ticket { Id = 103, Type = "VIP", Price = 1800 }, new Ticket { Id = 104, Type = "Workshop", Price = 400 } } }
                    }
                },
                new Event
                {
                    Id = 3,
                    Name = "AI & Data Summit",
                    Participants = new List<Participant>
                    {
                        new Participant { Id = 4, Name = "Максим", Tickets = new List<Ticket> { new Ticket { Id = 105, Type = "Standard", Price = 900 } } }
                    }
                },
                new Event
                {
                    Id = 4,
                    Name = "CyberSecurity Forum",
                    Participants = new List<Participant>
                    {
                        new Participant { Id = 5, Name = "Анна", Tickets = new List<Ticket> { new Ticket { Id = 106, Type = "Student", Price = 300 } } }
                    }
                }
            };

            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №16: SelectMany, Пагінація, Extension Methods ===\n");

            Console.WriteLine("--- 1. SELECTMANY: Отримати всі квитки в одному списку ---");
            var allTickets = events
                .SelectMany(e => e.Participants)
                .SelectMany(p => p.Tickets);

            foreach (var ticket in allTickets)
            {
                Console.WriteLine($"Квиток ID: {ticket.Id} | Тип: {ticket.Type} | Ціна: {ticket.Price} грн");
            }

            Console.WriteLine("\n--- 2. ПАГІНАЦІЯ: Події по 2 на сторінку ---");
            int pageSize = 2;
            int totalPages = (int)Math.Ceiling((double)events.Count / pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                Console.WriteLine($"\n--- Сторінка {page} ---");
                var pageEvents = events
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                foreach (var ev in pageEvents)
                {
                    Console.WriteLine($"Подія [{ev.Id}]: {ev.Name} (Учасників: {ev.Participants.Count})");
                }
            }

            Console.WriteLine("\n--- 3. CUSTOM EXTENSION METHOD: Загальний дохід від квитків ---");
            double totalRevenue = events.TotalTicketRevenue();
            Console.WriteLine($"Загальний дохід від усіх проданих квитків: {totalRevenue} грн");
        }
    }
}