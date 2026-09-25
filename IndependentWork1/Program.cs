using System;

namespace IndependentWork1
{
    public class Employee
    {
        // 2 приватні поля
        private string _fullName;
        private double _monthlySalary;

        // Властивість з get та set
        public string FullName
        {
            get => _fullName;
            set => _fullName = value;
        }

        public double MonthlySalary => _monthlySalary;

        public Employee(string fullName, double monthlySalary)
        {
            _fullName = fullName;
            _monthlySalary = monthlySalary;
        }

        public double CalculateAnnualSalary()
        {
            return _monthlySalary * 12;
        }
    }

    public class Rectangle
    {
        // 2 приватні поля
        private double _width;
        private double _height;

        public double Width
        {
            get => _width;
            set => _width = value > 0 ? value : 1;
        }

        public double Height
        {
            get => _height;
            set => _height = value > 0 ? value : 1;
        }

        public double Perimeter => 2 * (_width + _height);

        public Rectangle(double width, double height)
        {
            _width = width;
            _height = height;
        }

        public double CalculateArea()
        {
            return _width * _height;
        }
    }

    public class Recipe
    {
        private string _title;
        private int _cookingTimeMinutes;

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public int CookingTimeMinutes => _cookingTimeMinutes;

        public Recipe(string title, int cookingTimeMinutes)
        {
            _title = title;
            _cookingTimeMinutes = cookingTimeMinutes;
        }

        public bool IsQuickRecipe()
        {
            return _cookingTimeMinutes <= 30;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №1 ===\n");

            Console.WriteLine("--- 1. Клас Employee ---");
            Employee emp = new Employee("Олена Ковальчук", 25000);
            Console.WriteLine($"Працівник: {emp.FullName}");
            Console.WriteLine($"Місячна зарплата: {emp.MonthlySalary} грн");
            Console.WriteLine($"Річна зарплата: {emp.CalculateAnnualSalary()} грн\n");

            Console.WriteLine("--- 2. Клас Rectangle ---");
            Rectangle rect = new Rectangle(5.5, 4.0);
            Console.WriteLine($"Прямокутник {rect.Width}x{rect.Height}");
            Console.WriteLine($"Периметр: {rect.Perimeter}");
            Console.WriteLine($"Площа: {rect.CalculateArea()} кв. од.\n");

            Console.WriteLine("--- 3. Клас Recipe ---");
            Recipe recipe = new Recipe("Паста Карбонара", 20);
            Console.WriteLine($"Рецепт: {recipe.Title}");
            Console.WriteLine($"Час приготування: {recipe.CookingTimeMinutes} хв");
            Console.WriteLine($"Швидка страва? {(recipe.IsQuickRecipe() ? "Так" : "Ні")}");
        }
    }
}