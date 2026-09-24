using System;

namespace Lab6v11
{
    // Базовий клас
    public class Instrument
    {
        public string Brand { get; set; }
        public double Price { get; set; }

        public Instrument(string brand, double price)
        {
            Brand = brand;
            Price = price;
        }

        public virtual void PlaySound()
        {
            Console.WriteLine($"[Instrument] Грає базовий музичний інструмент ({Brand}).");
        }

        public string GetInstrumentType()
        {
            return "Звичайний музичний інструмент";
        }
    }

    public class Guitar : Instrument
    {
        public int NumStrings { get; set; }

        public Guitar(string brand, double price, int numStrings) 
            : base(brand, price)
        {
            NumStrings = numStrings;
        }

        public override void PlaySound()
        {
            Console.WriteLine($"[Guitar] Гітара {Brand} ({NumStrings} струн) бринчить: Дринь-дринь!");
        }

        public void Strum()
        {
            Console.WriteLine($"[Guitar] Виконується прийом бренчання (Strum) на гітарі {Brand}.");
        }

        public new string GetInstrumentType()
        {
            return "Струнний інструмент (Гітара)";
        }
    }

    public class Piano : Instrument
    {
        public int NumKeys { get; set; }

        public Piano(string brand, double price, int numKeys) 
            : base(brand, price)
        {
            NumKeys = numKeys;
        }

        public override void PlaySound()
        {
            Console.WriteLine($"[Piano] Піаніно {Brand} ({NumKeys} клавіш) лунає: До-Ре-Мі!");
        }

        public void PressPedal()
        {
            Console.WriteLine($"[Piano] Натиснуто педаль сустейну на піаніно {Brand}.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №6 (Варіант 11: Instrument -> Guitar -> Piano) ===\n");

            Instrument genericInstrument = new Instrument("Yamaha Generic", 300);
            Guitar guitar = new Guitar("Fender", 1200, 6);
            Piano piano = new Piano("Steinway", 15000, 88);

            Console.WriteLine("--- 1. Демонстрація поліморфізму (через масив Instrument[]) ---");
            Instrument[] orchestra = new Instrument[] { genericInstrument, guitar, piano };

            foreach (var inst in orchestra)
            {
                inst.PlaySound();
            }

            Console.WriteLine("\n--- 2. Демонстрація унікальних методів ---");
            guitar.Strum();
            piano.PressPedal();

            Console.WriteLine("\n--- 3. Демонстрація різниці між override та new ---");
            
            Console.WriteLine($"Виклик через Guitar: {guitar.GetInstrumentType()}");

            Instrument guitarAsInstrument = guitar;
            Console.WriteLine($"Виклик через Instrument (для Guitar): {guitarAsInstrument.GetInstrumentType()}");

            Console.WriteLine("\nПояснення: override змінює поведінку незалежно від типу посилання, а new лише приховує метод базового класу при зверненні через тип похідного класу.");
        }
    }
} 