using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        private static readonly HashSet<string> StopWords = new HashSet<string>
        {
            "a", "an", "the", "is", "are", "was", "were",
            "in", "on", "at", "to", "for", "of", "and", "or"
        };

        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Enumerable.Empty<string>();

            string lowerText = text.ToLowerInvariant();
            var matches = Regex.Matches(lowerText, @"[a-zа-яіїєґ0-9+#]+");

            return matches.Select(m => m.Value);
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            return words.Where(w => !StopWords.Contains(w));
        }
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index;
        private readonly Dictionary<int, Document> _documents;

        public InvertedIndex()
        {
            _index = new Dictionary<string, HashSet<int>>();
            _documents = new Dictionary<int, Document>();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;

            var tokens = TextProcessor.Tokenize(doc.Content);
            var cleanWords = TextProcessor.RemoveStopWords(tokens);

            foreach (var word in cleanWords)
            {
                if (!_index.ContainsKey(word))
                {
                    _index[word] = new HashSet<int>();
                }
                _index[word].Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var normalized = TextProcessor.Tokenize(word).FirstOrDefault();
            if (string.IsNullOrEmpty(normalized) || !_index.TryGetValue(normalized, out var docIds))
            {
                return new List<Document>();
            }

            return docIds.Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            var normalizedWords = words
                .SelectMany(w => TextProcessor.Tokenize(w))
                .Distinct()
                .ToList();

            if (normalizedWords.Count == 0)
                return new List<Document>();

            HashSet<int>? resultIds = null;

            foreach (var word in normalizedWords)
            {
                if (!_index.TryGetValue(word, out var docIds))
                {
                    return new List<Document>();
                }

                if (resultIds == null)
                {
                    resultIds = new HashSet<int>(docIds);
                }
                else
                {
                    resultIds.IntersectWith(docIds);
                }
            }

            return resultIds == null
                ? new List<Document>()
                : resultIds.Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            var normalizedWords = words
                .SelectMany(w => TextProcessor.Tokenize(w))
                .Distinct();

            var resultIds = new HashSet<int>();

            foreach (var word in normalizedWords)
            {
                if (_index.TryGetValue(word, out var docIds))
                {
                    resultIds.UnionWith(docIds);
                }
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }
    }

    public static class PerformanceTest
    {
        public static List<Document> GenerateDocuments(int count)
        {
            var baseTemplates = new[]
            {
                ("Introduction to C#", "C# is a modern object-oriented programming language developed by Microsoft."),
                ("Java Basics", "Java is a popular programming language known for its portability and performance."),
                ("Python Overview", "Python is a high-level programming language with simple syntax."),
                ("C# Advanced Features", "C# supports LINQ, async/await, and many modern programming paradigms."),
                ("Comparing Languages", "C#, Java, and Python are among the most popular programming languages today.")
            };

            var extraWords = new[] { "cloud", "database", "network", "security", "algorithm", "compiler", "memory", "thread", "docker", "microservice" };
            var random = new Random(42);
            var list = new List<Document>(count);

            for (int i = 1; i <= count; i++)
            {
                var template = baseTemplates[(i - 1) % baseTemplates.Length];
                string extra = i <= 5
                    ? ""
                    : $" Extra topics: {extraWords[random.Next(extraWords.Length)]} {extraWords[random.Next(extraWords.Length)]}.";

                list.Add(new Document
                {
                    Id = i,
                    Title = i <= 5 ? template.Item1 : $"{template.Item1} (Part {i})",
                    Content = template.Item2 + extra
                });
            }

            return list;
        }

        public static void CompareSearchMethods(List<Document> documents, string searchWord)
        {
            var sw = new Stopwatch();
            var normalizedWord = TextProcessor.Tokenize(searchWord).First();

            _ = documents.Take(1).Where(d => TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).ToList();

            sw.Start();
            var linearResults = documents
                .Where(d => TextProcessor.Tokenize(d.Content).Contains(normalizedWord))
                .ToList();
            sw.Stop();
            long linearTicks = sw.ElapsedTicks;

            sw.Restart();
            var index = new InvertedIndex();
            foreach (var doc in documents)
            {
                index.AddDocument(doc);
            }
            sw.Stop();
            long buildTicks = sw.ElapsedTicks;

            sw.Restart();
            var indexResults = index.SearchSingleWord(searchWord);
            sw.Stop();
            long indexTicks = sw.ElapsedTicks;

            double speedup = indexTicks > 0 ? (double)linearTicks / indexTicks : 0;

            Console.WriteLine($"Документів: {documents.Count,6} | Linear: {linearTicks,8} ticks | Index Search: {indexTicks,6} ticks | Прискорення: {speedup,6:F1}x | Знайдено: {linearResults.Count}/{indexResults.Count}");
        }
    }

    internal class Program
    {
        static List<Document> GetInitialDocuments()
        {
            return new List<Document>
            {
                new Document
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    Content = "C# is a modern object-oriented programming language developed by Microsoft."
                },
                new Document
                {
                    Id = 2,
                    Title = "Java Basics",
                    Content = "Java is a popular programming language known for its portability and performance."
                },
                new Document
                {
                    Id = 3,
                    Title = "Python Overview",
                    Content = "Python is a high-level programming language with simple syntax."
                },
                new Document
                {
                    Id = 4,
                    Title = "C# Advanced Features",
                    Content = "C# supports LINQ, async/await, and many modern programming paradigms."
                },
                new Document
                {
                    Id = 5,
                    Title = "Comparing Languages",
                    Content = "C#, Java, and Python are among the most popular programming languages today."
                }
            };
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ЧАСТИНА 1: Вимірювання продуктивності (Linear Search vs Inverted Index) ===");
            int[] testSizes = { 10, 100, 1000, 10000 };
            foreach (int size in testSizes)
            {
                var testDocs = PerformanceTest.GenerateDocuments(size);
                PerformanceTest.CompareSearchMethods(testDocs, "python");
            }

            Console.WriteLine("\n=== ЧАСТИНА 2: Інтерактивна демонстрація пошуку ===");
            var index = new InvertedIndex();
            foreach (var doc in GetInitialDocuments())
            {
                index.AddDocument(doc);
            }

            Console.WriteLine($"Loaded {index.DocumentCount} documents");
            Console.WriteLine($"Index contains {index.UniqueWords} unique words\n");

            // Демонстрація OR пошуку
            var anyResults = index.SearchAnyWord("java", "python");
            Console.WriteLine($"[Demo OR-пошук 'java' OR 'python']: знайдено {anyResults.Count} документів -> {string.Join(", ", anyResults)}\n");

            while (true)
            {
                Console.Write("Enter search query (or 'exit'): ");
                var query = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(query) || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                Console.WriteLine("\n--- Search Results (AND) ---");
                var results = index.SearchMultipleWords(words);
                foreach (var doc in results)
                {
                    Console.WriteLine(doc);
                    Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
                }

                Console.WriteLine($"\nFound {results.Count} documents\n");
            }
        }
    }
}