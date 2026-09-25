using System;

namespace IndependentWork2
{
    public class Product
    {
        private int _id;
        private string _name;
        private decimal _price;
        private string _category;
        private int _stockCount;


        public int Id => _id;
        public string Name => _name;
        public decimal Price => _price;
        public string Category => _category;
        public int StockCount => _stockCount;

        public Product(int id, string name, decimal price, string category, int stockCount)
        {
            _id = id;
            _name = name;
            _price = price;
            _category = category;
            _stockCount = stockCount;
        }

        public Product(int id, string name, decimal price) 
            : this(id, name, price, "Uncategorized", 0)
        {
        }

        public Product(Product other) 
            : this(other.Id, other.Name, other.Price, other.Category, other.StockCount)
        {
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Price: {Price:C}, Category: {Category}, Stock: {StockCount}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №2 ===\n");
            Console.WriteLine("Створення товарів:");

            Product p1 = new Product(101, "Laptop", 35000.00m, "Electronics", 10);
            Console.WriteLine($"Товар 1 (основний конструктор): {p1}");

            Product p2 = new Product(102, "Mouse", 880.00m);
            Console.WriteLine($"Товар 2 (скорочений конструктор): {p2}");

            Product p3 = new Product(p1);
            Console.WriteLine($"Товар 3 (конструктор копіювання): {p3}");
        }
    }
}