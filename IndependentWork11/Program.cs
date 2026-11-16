using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork11
{
    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== САМОСТІЙНА РОБОТА №12: LINQ Pipelines (ETL та аналітика) ===\n");

            var (products, customers, sales) = GenerateTestData(seed: 42);
            Console.WriteLine($"Завантажено даних: {products.Count} товарів, {customers.Count} клієнтів, {sales.Count} продажів.\n");

            TopProductsByCategory(sales, products);
            TotalRevenueByCity(sales, products, customers);
            SalesByMonthAndYear(sales, products);
            VipCustomersLoyaltyReport(sales, products, customers);
        }

        static (List<Product> Products, List<Customer> Customers, List<Sale> Sales) GenerateTestData(int seed)
        {
            var random = new Random(seed);
            string[] categories = { "Ноутбуки", "Смартфони", "Периферія", "Монітори", "Аудіо" };
            string[] cities = { "Рівне", "Київ", "Львів", "Дніпро", "Одеса", "Харків" };
            string[] firstNames = { "Олена", "Іван", "Софія", "Максим", "Анна", "Дмитро", "Марія", "Артем" };
            string[] lastNames = { "Ковальчук", "Бондаренко", "Мельник", "Ткаченко", "Шевченко", "Кравчук" };

            var products = new List<Product>();
            for (int i = 1; i <= 60; i++)
            {
                string category = categories[i % categories.Length];
                decimal basePrice = category switch
                {
                    "Ноутбуки" => random.Next(22000, 55000),
                    "Смартфони" => random.Next(9000, 38000),
                    "Монітори" => random.Next(6500, 19000),
                    "Аудіо" => random.Next(1200, 8500),
                    _ => random.Next(500, 4500)
                };

                products.Add(new Product
                {
                    Id = i,
                    Name = $"{category[..^1]} Модель #{i:D2}",
                    Category = category,
                    UnitPrice = basePrice
                });
            }

            var customers = new List<Customer>();
            for (int i = 1; i <= 70; i++)
            {
                string fullName = $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
                customers.Add(new Customer
                {
                    Id = i,
                    Name = $"{fullName} (#{i})",
                    City = cities[random.Next(cities.Length)]
                });
            }

            var sales = new List<Sale>();
            var startDate = new DateTime(2025, 1, 1);
            for (int i = 1; i <= 150; i++)
            {
                sales.Add(new Sale
                {
                    Id = i,
                    ProductId = random.Next(1, products.Count + 1),
                    CustomerId = random.Next(1, customers.Count + 1),
                    Quantity = random.Next(1, 6),
                    SaleDate = startDate.AddDays(random.Next(0, 650))
                });
            }

            return (products, customers, sales);
        }

        static void TopProductsByCategory(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var topByCategory = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity)
                })
                .Join(
                    products,
                    stat => stat.ProductId,
                    prod => prod.Id,
                    (stat, prod) => new
                    {
                        prod.Category,
                        prod.Name,
                        stat.TotalQuantity
                    })
                .GroupBy(x => x.Category)
                .Select(catGroup => new
                {
                    Category = catGroup.Key,
                    Top3 = catGroup
                        .OrderByDescending(x => x.TotalQuantity)
                        .Take(3)
                        .ToList()
                })
                .OrderBy(c => c.Category)
                .ToList();

            sw.Stop();

            Console.WriteLine($"--- Пайплайн 1: Топ-3 найпопулярніші продукти за категоріями (Час: {sw.Elapsed.TotalMilliseconds:F3} мс / {sw.ElapsedTicks} ticks) ---");
            foreach (var group in topByCategory)
            {
                Console.WriteLine($"  Категорія [{group.Category}]:");
                foreach (var item in group.Top3)
                {
                    Console.WriteLine($"    - {item.Name}: продано {item.TotalQuantity} од.");
                }
            }
            Console.WriteLine();
        }

        static void TotalRevenueByCity(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var revenueByCity = sales
                .Join(
                    products,
                    s => s.ProductId,
                    p => p.Id,
                    (s, p) => new
                    {
                        s.CustomerId,
                        Revenue = s.Quantity * p.UnitPrice
                    })
                .Join(
                    customers,
                    sp => sp.CustomerId,
                    c => c.Id,
                    (sp, c) => new
                    {
                        c.City,
                        sp.Revenue
                    })
                .GroupBy(x => x.City)
                .Select(g => new
                {
                    City = g.Key,
                    TotalRevenue = g.Sum(x => x.Revenue),
                    OrdersCount = g.Count()
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();

            Console.WriteLine($"--- Пайплайн 2: Загальний дохід за містами клієнтів (Час: {sw.Elapsed.TotalMilliseconds:F3} мс / {sw.ElapsedTicks} ticks) ---");
            foreach (var item in revenueByCity)
            {
                Console.WriteLine($"  * {item.City,-8} | Замовлень: {item.OrdersCount,2} | Дохід: {item.TotalRevenue,12:N2} грн");
            }
            Console.WriteLine();
        }

        static void SalesByMonthAndYear(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var monthlyStats = sales
                .Join(
                    products,
                    s => s.ProductId,
                    p => p.Id,
                    (s, p) => new
                    {
                        s.SaleDate.Year,
                        s.SaleDate.Month,
                        s.Quantity,
                        Revenue = s.Quantity * p.UnitPrice
                    })
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalUnits = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .Take(8) 
                .ToList();

            sw.Stop();

            Console.WriteLine($"--- Пайплайн 3: Динаміка продажів за роками та місяцями (Час: {sw.Elapsed.TotalMilliseconds:F3} мс / {sw.ElapsedTicks} ticks) ---");
            foreach (var m in monthlyStats)
            {
                Console.WriteLine($"  * {m.Year}-{m.Month:D2} | Продано одиниць: {m.TotalUnits,3} | Загальний дохід: {m.TotalRevenue,12:N2} грн");
            }
            Console.WriteLine();
        }

        static void VipCustomersLoyaltyReport(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var vipCustomers = sales
                .Where(s => s.SaleDate.Year == 2026)
                .Join(
                    products,
                    s => s.ProductId,
                    p => p.Id,
                    (s, p) => new
                    {
                        s.CustomerId,
                        p.Category,
                        Amount = s.Quantity * p.UnitPrice
                    })
                .Join(
                    customers,
                    sp => sp.CustomerId,
                    c => c.Id,
                    (sp, c) => new
                    {
                        c.Id,
                        c.Name,
                        c.City,
                        sp.Category,
                        sp.Amount
                    })
                .GroupBy(x => new { x.Id, x.Name, x.City })
                .Select(g => new
                {
                    g.Key.Name,
                    g.Key.City,
                    TotalSpent = g.Sum(x => x.Amount),
                    UniqueCategories = g.Select(x => x.Category).Distinct().Count()
                })
                .Where(c => c.TotalSpent > 30000m)
                .OrderByDescending(c => c.TotalSpent)
                .Take(5)
                .ToList();

            sw.Stop();

            Console.WriteLine($"--- Пайплайн 4: Топ-5 VIP-клієнтів 2026 року (> 30 000 грн) (Час: {sw.Elapsed.TotalMilliseconds:F3} мс / {sw.ElapsedTicks} ticks) ---");
            foreach (var vip in vipCustomers)
            {
                Console.WriteLine($"  * {vip.Name,-24} ({vip.City,-6}) | Категорій: {vip.UniqueCategories} | Витрачено: {vip.TotalSpent,12:N2} грн");
            }
            Console.WriteLine();
        }
    }
}