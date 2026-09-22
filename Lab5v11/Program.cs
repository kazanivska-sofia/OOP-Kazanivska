using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lab5v11
{
    public class ColorPalette : IEnumerable<string>, IEquatable<ColorPalette>
    {
        private readonly List<string> _colors = new List<string>();

        public int Count => _colors.Count;

        public ColorPalette() { }

        public ColorPalette(IEnumerable<string> colors)
        {
            if (colors != null)
            {
                _colors.AddRange(colors.Where(c => !string.IsNullOrWhiteSpace(c)));
            }
        }

        public void AddColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
            {
                throw new ArgumentException("Назва кольору не може бути порожньою!");
            }
            _colors.Add(color);
        }

        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= _colors.Count)
                {
                    throw new IndexOutOfRangeException($"Індекс {index} виходить за межі палітри (розмір: {_colors.Count}).");
                }
                return _colors[index];
            }
            set
            {
                if (index < 0 || index >= _colors.Count)
                {
                    throw new IndexOutOfRangeException($"Індекс {index} виходить за межі палітри (розмір: {_colors.Count}).");
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Назва кольору не може бути порожньою!");
                }
                _colors[index] = value;
            }
        }

        public static ColorPalette operator +(ColorPalette p1, ColorPalette p2)
        {
            if (p1 is null && p2 is null) return new ColorPalette();
            if (p1 is null) return new ColorPalette(p2._colors);
            if (p2 is null) return new ColorPalette(p1._colors);

            ColorPalette newPalette = new ColorPalette(p1._colors);
            foreach (var color in p2._colors)
            {
                if (!newPalette._colors.Contains(color, StringComparer.OrdinalIgnoreCase))
                {
                    newPalette.AddColor(color);
                }
            }
            return newPalette;
        }

        public static bool operator ==(ColorPalette? p1, ColorPalette? p2)
        {
            if (ReferenceEquals(p1, p2)) return true;
            if (p1 is null || p2 is null) return false;
            return p1.Equals(p2);
        }

        public static bool operator !=(ColorPalette? p1, ColorPalette? p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object? obj)
        {
            return obj is ColorPalette palette && Equals(palette);
        }

        public bool Equals(ColorPalette? other)
        {
            if (other is null) return false;
            return _colors.SequenceEqual(other._colors, StringComparer.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var color in _colors)
            {
                hash = hash * 31 + color.ToLowerInvariant().GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            if (_colors.Count == 0) return "Палітра порожня";
            return $"Палітра [{_colors.Count} кольорів]: " + string.Join(", ", _colors);
        }

        public IEnumerator<string> GetEnumerator() => _colors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №5 (Варіант 11: ColorPalette) ===\n");

            ColorPalette palette1 = new ColorPalette();
            palette1.AddColor("Red");
            palette1.AddColor("Green");
            palette1.AddColor("Blue");

            ColorPalette palette2 = new ColorPalette(new[] { "Yellow", "Blue", "Purple" });

            Console.WriteLine($"Палітра 1: {palette1}");
            Console.WriteLine($"Палітра 2: {palette2}");

            Console.WriteLine("\n--- Демонстрація індексатора ---");
            Console.WriteLine($"Перший колір палітри 1 (palette1[0]): {palette1[0]}");

            Console.WriteLine("Змінюємо palette1[1] з 'Green' на 'Emerald'...");
            palette1[1] = "Emerald";
            Console.WriteLine($"Оновлена палітра 1: {palette1}");

            Console.WriteLine("\n--- Перевантаження оператора + (об'єднання палітр) ---");
            ColorPalette combined = palette1 + palette2;
            Console.WriteLine($"Об'єднана палітра (palette1 + palette2): {combined}");

            Console.WriteLine("\n--- Перевантаження операторів == та != ---");
            ColorPalette palette3 = new ColorPalette(new[] { "Red", "Emerald", "Blue" });
            Console.WriteLine($"palette1 == palette3? {palette1 == palette3}");
            Console.WriteLine($"palette1 == palette2? {palette1 == palette2}");
        }
    }
}