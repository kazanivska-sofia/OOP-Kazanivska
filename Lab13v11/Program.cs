using System;
using System.Collections.Generic;

namespace Lab13v11
{
    // Власний клас для тестування
    public class Movie
    {
        public string Title { get; set; }
        public double Rating { get; set; }

        public override string ToString() => $"{Title} (Rating: {Rating})";

        public override bool Equals(object obj)
        {
            if (obj is Movie other)
                return Title == other.Title && Rating == other.Rating;
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Title, Rating);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №13 (Варіант 11) ===\n");

            // 1. Робота з примітивними типами (int)
            Console.WriteLine("--- 1. Тестування з int ---");
            var numbers = new List<int> { 1, 2, 3, 4, 2, 5, 3 };

            var squared = FunctionalUtils.Map(numbers, x => x * x);
            Console.WriteLine($"Map (x^2): {string.Join(", ", squared)}");

            var sum = FunctionalUtils.Reduce(numbers, 0, (acc, x) => acc + x);
            Console.WriteLine($"Reduce (Sum): {sum}");

            var evenCount = FunctionalUtils.Count(numbers, x => x % 2 == 0);
            Console.WriteLine($"Count (Парні): {evenCount}");

            var avgNum = FunctionalUtils.Average(numbers, x => x);
            Console.WriteLine($"Average: {avgNum:F2}");

            var uniqueNums = FunctionalUtils.Distinct(numbers);
            Console.WriteLine($"Distinct: {string.Join(", ", uniqueNums)}\n");

            // 2. Робота з рядками (string)
            Console.WriteLine("--- 2. Тестування зі string ---");
            var words = new List<string> { "Apple", "Banana", "Apple", "Cherry", "Date" };

            var upperWords = FunctionalUtils.Map(words, s => s.ToUpper());
            Console.WriteLine($"Map (ToUpper): {string.Join(", ", upperWords)}");

            var longWordsCount = FunctionalUtils.Count(words, s => s.Length > 5);
            Console.WriteLine($"Count (довжина > 5): {longWordsCount}");

            var avgLength = FunctionalUtils.Average(words, s => s.Length);
            Console.WriteLine($"Average (довжина слів): {avgLength:F2}");

            var uniqueWords = FunctionalUtils.Distinct(words);
            Console.WriteLine($"Distinct: {string.Join(", ", uniqueWords)}\n");

            // 3. Робота з власним класом (Movie)
            Console.WriteLine("--- 3. Тестування з власним класом (Movie) ---");
            var movies = new List<Movie>
            {
                new Movie { Title = "Inception", Rating = 8.8 },
                new Movie { Title = "Interstellar", Rating = 8.6 },
                new Movie { Title = "The Dark Knight", Rating = 9.0 },
                new Movie { Title = "Inception", Rating = 8.8 }
            };

            var titles = FunctionalUtils.Map(movies, m => m.Title);
            Console.WriteLine($"Map (Назви фільмів): {string.Join(", ", titles)}");

            var highRatedCount = FunctionalUtils.Count(movies, m => m.Rating >= 8.8);
            Console.WriteLine($"Count (Rating >= 8.8): {highRatedCount}");

            var avgRating = FunctionalUtils.Average(movies, m => m.Rating);
            Console.WriteLine($"Average Rating: {avgRating:F2}");

            var uniqueMovies = FunctionalUtils.Distinct(movies);
            Console.WriteLine($"Distinct (унікальні об'єкти): {string.Join(", ", uniqueMovies)}");
        }
    }
}