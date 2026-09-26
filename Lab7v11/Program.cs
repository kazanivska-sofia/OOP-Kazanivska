using System;

namespace Lab7v11
{
    // Базовий клас
    public class Report
    {
        public string Title { get; set; }

        public Report(string title)
        {
            Title = title;
        }

        // Віртуальний метод для перевизначення/приховування
        public virtual void Generate()
        {
            Console.WriteLine($"[Report] Звичайний звіт: '{Title}'");
        }
    }

    // Похідний клас A (використовує override)
    public class AnnualReport : Report
    {
        public int Year { get; set; }

        public AnnualReport(string title, int year) : base(title)
        {
            Year = year;
        }

        // Перевизначення методу (Поліморфізм)
        public override void Generate()
        {
            Console.WriteLine($"[AnnualReport] Річний звіт за {Year} рік: '{Title}'");
        }
    }

    // Похідний клас B (використовує new)
    public class MonthlyReport : Report
    {
        public string Month { get; set; }

        public MonthlyReport(string title, string month) : base(title)
        {
            Month = month;
        }

        // Приховування методу базового класу
        public new void Generate()
        {
            Console.WriteLine($"[MonthlyReport] Щомісячний звіт за {Month}: '{Title}'");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №7 (Варіант 11) ===");
            Console.WriteLine("Приховування методів (new) vs Перевизначення (override)\n");

            // 1. Створення об'єктів
            AnnualReport annual = new AnnualReport("Фінансовий підсумок", 2025);
            MonthlyReport monthly = new MonthlyReport("Продажі за березень", "Березень");

            // 2. Upcasting (збереження у змінних базового типу Report)
            Report report1 = annual;
            Report report2 = monthly;

            // 3. Виклик методів через посилання базового типу
            Console.WriteLine("--- 1. Виклик через посилання базового типу (Report) ---");
            report1.Generate(); // Викличе AnnualReport.Generate() за рахунок override (динамічний зв'язок)
            report2.Generate(); // Викличе Report.Generate() через new (статичний зв'язок)


            Console.WriteLine("\n--- 2. Виклик через посилання похідних типів ---");
            ((AnnualReport)report1).Generate();  // AnnualReport.Generate()
            ((MonthlyReport)report2).Generate(); // MonthlyReport.Generate()
        }
    }
}