using System;

namespace Lab4v11
{
    public class Duration : IEquatable<Duration>
    {
        private int _totalSeconds;

        public int TotalSeconds
        {
            get => _totalSeconds;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Тривалість має бути більшою за 0 секунд!");
                }
                _totalSeconds = value;
            }
        }

        public int Hours => _totalSeconds / 3600;
        public int Minutes => (_totalSeconds % 3600) / 60;
        public int Seconds => _totalSeconds % 60;

        public Duration(int totalSeconds)
        {
            TotalSeconds = totalSeconds;
        }

        public static Duration FromMinutes(int minutes)
        {
            if (minutes <= 0)
            {
                throw new ArgumentException("Кількість хвилин має бути більшою за 0!");
            }
            return new Duration(minutes * 60);
        }

        public int this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Hours,
                    1 => Minutes,
                    2 => Seconds,
                    _ => throw new IndexOutOfRangeException("Індекс має бути від 0 до 2 (0: Години, 1: Хвилини, 2: Секунди).")
                };
            }
        }

        public static Duration operator +(Duration d1, Duration d2)
        {
            if (d1 is null || d2 is null)
                throw new ArgumentNullException("Об'єкти Duration не можуть бути null.");

            return new Duration(d1.TotalSeconds + d2.TotalSeconds);
        }

        public static bool operator >(Duration d1, Duration d2)
        {
            if (d1 is null || d2 is null) return false;
            return d1.TotalSeconds > d2.TotalSeconds;
        }

        public static bool operator <(Duration d1, Duration d2)
        {
            if (d1 is null || d2 is null) return false;
            return d1.TotalSeconds < d2.TotalSeconds;
        }

        public static bool operator ==(Duration d1, Duration d2)
        {
            if (ReferenceEquals(d1, d2)) return true;
            if (d1 is null || d2 is null) return false;
            return d1.Equals(d2);
        }

        public static bool operator !=(Duration d1, Duration d2)
        {
            return !(d1 == d2);
        }

        public override bool Equals(object? obj)
        {
            return obj is Duration duration && Equals(duration);
        }

        public bool Equals(Duration? other)
        {
            if (other is null) return false;
            return _totalSeconds == other._totalSeconds;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_totalSeconds);
        }

        public override string ToString()
        {
            TimeSpan t = TimeSpan.FromSeconds(_totalSeconds);
            return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2} ({_totalSeconds} сек)";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №4 (Варіант 11: Duration) ===\n");

            Console.WriteLine("--- 1. Створення об'єктів та валідація ---");
            Duration d1 = new Duration(3665); // 1 година, 1 хвилина, 5 секунд
            Duration d2 = Duration.FromMinutes(45); // Статичний метод (2700 сек)

            Console.WriteLine($"d1: {d1}");
            Console.WriteLine($"d2 (створено з FromMinutes(45)): {d2}");

            try
            {
                Console.WriteLine("Спроба створити Duration з від'ємним значенням (-100)...");
                Duration dInvalid = new Duration(-100);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[Валідація спрацювала]: {ex.Message}");
            }

            Console.WriteLine("\n--- 2. Демонстрація індексатора ---");
            Console.WriteLine($"Для d1 ({d1}):");
            Console.WriteLine($"[0] Години: {d1[0]}");
            Console.WriteLine($"[1] Хвилини: {d1[1]}");
            Console.WriteLine($"[2] Секунди: {d1[2]}");

            Console.WriteLine("\n--- 3. Перевантажені оператори (+, >, ==) ---");
            Duration sum = d1 + d2;
            Console.WriteLine($"Сума (d1 + d2): {sum}");

            Console.WriteLine($"Чи d1 > d2? {d1 > d2}");

            Duration d3 = new Duration(3665);
            Console.WriteLine($"d1 == d3? {d1 == d3}");
            Console.WriteLine($"d1 == d2? {d1 == d2}");
        }
    }
}