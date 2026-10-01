using System;
using System.Collections.Generic;

namespace Lab8v11
{
    public class Building
    {
        public string Address { get; set; }

        public Building(string address)
        {
            Address = address;
        }

        public virtual void GetPurpose()
        {
            Console.WriteLine($"Будівля за адресою '{Address}': загальне призначення.");
        }
    }

    public class House : Building
    {
        public int NumRooms { get; set; }

        public House(string address, int numRooms) : base(address)
        {
            NumRooms = numRooms;
        }

        public override void GetPurpose()
        {
            Console.WriteLine($"Житловий будинок на {Address}: призначений для проживання ({NumRooms} кімнат).");
        }
    }

    public class Office : Building
    {
        public int NumFloors { get; set; }

        public Office(string address, int numFloors) : base(address)
        {
            NumFloors = numFloors;
        }

        public override void GetPurpose()
        {
            Console.WriteLine($"Офісний центр на {Address}: призначений для роботи бізнесу ({NumFloors} поверхів).");
        }
    }

    public class Shop : Building
    {
        public string ProductType { get; set; }

        public Shop(string address, string productType) : base(address)
        {
            ProductType = productType;
        }

        public override void GetPurpose()
        {
            Console.WriteLine($"Магазин на {Address}: призначений для торгівлі (товари: {ProductType}).");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №8 (Варіант 11) ===");
            Console.WriteLine("Поліморфізм: динамічне зв'язування та перевизначення методів\n");

            List<Building> buildings = new List<Building>
            {
                new House("вул. Шевченка, 12", 4),
                new Office("просп. Незалежності, 50", 10),
                new Shop("вул. Соборна, 3", "Електроніка"),
                new House("вул. Лесі Українки, 8", 2),
                new Shop("вул. Київська, 105", "Продукти харчування")
            };

            Console.WriteLine("--- Список призначень усіх будівель (поліморфні виклики) ---");
            int count = 0;

            foreach (var building in buildings)
            {
                count++;
                Console.Write($"{count}. ");
                building.GetPurpose();
            }

            Console.WriteLine("\n--- Агрегація результатів ---");
            Console.WriteLine($"Всього оброблено будівель у місті: {buildings.Count}");
        }
    }
}