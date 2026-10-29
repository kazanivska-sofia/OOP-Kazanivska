using System;
using System.Collections.Generic;
using System.Linq;

namespace lab12v11
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public interface IRepository<T> where T : IEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    public class Movie : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
    }

    public class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly List<T> _items = new();

        public T GetById(int id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null)
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено.");
            return item;
        }

        public IEnumerable<T> GetAll() => _items.ToList();

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (_items.Any(x => x.Id == item.Id))
                throw new InvalidOperationException($"Елемент з ID {item.Id} вже існує.");
            _items.Add(item);
        }

        public void Update(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var existingIndex = _items.FindIndex(x => x.Id == item.Id);
            if (existingIndex == -1)
                throw new KeyNotFoundException($"Елемент з ID {item.Id} не знайдено для оновлення.");
            _items[existingIndex] = item;
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            _items.Remove(item);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №12 (Варіант 11: Movie) ===");

            var repo = new InMemoryRepository<Movie>();

            repo.Add(new Movie { Id = 1, Title = "Inception", Director = "Christopher Nolan", ReleaseYear = 2010 });
            repo.Add(new Movie { Id = 2, Title = "The Matrix", Director = "Lana Wachowski", ReleaseYear = 1999 });

            Console.WriteLine("\n--- Усі фільми у списку ---");
            foreach (var movie in repo.GetAll())
            {
                Console.WriteLine($"[{movie.Id}] {movie.Title} ({movie.ReleaseYear}) - Режисер: {movie.Director}");
            }
        }
    }
}
