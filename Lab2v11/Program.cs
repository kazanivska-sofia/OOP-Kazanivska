using System;

namespace Lab2v11
{
    public class Plane
    {
        private string _airline;
        private string _model;
        private int _capacity;

        public string Airline
        {
            get => _airline;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _airline = "Unknown Airline";
                }
                else
                {
                    _airline = value;
                }
            }
        }

        public string Model
        {
            get => _model;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _model = "Unknown Model";
                }
                else
                {
                    _model = value;
                }
            }
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value <= 0)
                {
                    Console.WriteLine("Помилка: місткість має бути > 0! Встановлено значення за замовчуванням (100).");
                    _capacity = 100;
                }
                else
                {
                    _capacity = value;
                }
            }
        }

        public Plane() : this("N/A", "Boeing 737", 150)
        {
            Console.WriteLine("[Виклик]: Викликано конструктор за замовчуванням (через : this()).");
        }

        public Plane(string airline, string model, int capacity)
        {
            Airline = airline;
            Model = model;
            Capacity = capacity;
            Console.WriteLine($"[Конструктор]: Створено літак {Model} компанії '{Airline}' на {Capacity} місць.");
        }

        public void Fly()
        {
            Console.WriteLine($"Літак {Model} авіакомпанії '{Airline}' виконує політ з {Capacity} пасажирами.");
        }

        ~Plane()
        {
            Console.WriteLine($"[Деструктор]: Об'єкт літака {Model} видалено з пам'яті.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Створення об'єктів ---");
            
            Plane plane1 = new Plane();
            plane1.Fly();

            Console.WriteLine();

            Plane plane2 = new Plane("Антонов", "Ан-178", 90);
            plane2.Fly();

            Console.WriteLine();

            Plane plane3 = new Plane("Wizz Air", "Airbus A321", -50);
            plane3.Fly();

            Console.WriteLine("\n--- Демонстрація роботи Garbage Collector ---");
            
            plane1 = null;
            plane2 = null;
            plane3 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Завершення програми.");
        }
    }
}