using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork9
{
    public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; } = string.Empty;

        public override string ToString() =>
            $"{FileName} ({Size} байт, змінено: {LastModified:yyyy-MM-dd HH:mm}, Hash: {Hash})";
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache =
            new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

        public void Add(string filePath, FileMetadata metadata)
        {
            _cache[filePath] = metadata;
        }

        public FileMetadata? Get(string filePath)
        {
            return _cache.TryGetValue(filePath, out var metadata) ? metadata : null;
        }

        public bool Remove(string filePath)
        {
            return _cache.Remove(filePath);
        }

        public bool Contains(string filePath)
        {
            return _cache.ContainsKey(filePath);
        }

        public int Count => _cache.Count;
    }

    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags =
            new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.ContainsKey(articleId))
            {
                _articleTags[articleId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            _articleTags[articleId].Add(tag);
        }

        public bool RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
            {
                return tags.Remove(tag);
            }
            return false;
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tags in _articleTags.Values)
            {
                allTags.UnionWith(tags);
            }
            return allTags;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (_articleTags.TryGetValue(articleId1, out var tags1) &&
                _articleTags.TryGetValue(articleId2, out var tags2))
            {
                var common = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
                common.IntersectWith(tags2);
                return common;
            }
            return new HashSet<string>();
        }
    }

    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex =
            new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            char[] separators = { ' ', '.', ',', '!', '?', ';', ':', '-', '\n', '\r', '\t' };
            var words = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (!_invertedIndex.ContainsKey(word))
                {
                    _invertedIndex[word] = new HashSet<int>();
                }
                _invertedIndex[word].Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            if (_invertedIndex.TryGetValue(word, out var docIds))
            {
                return new HashSet<int>(docIds);
            }
            return new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words == null || words.Length == 0)
            {
                return new HashSet<int>();
            }

            HashSet<int>? result = null;

            foreach (var word in words)
            {
                var docsForWord = Search(word);
                if (result == null)
                {
                    result = docsForWord;
                }
                else
                {
                    result.IntersectWith(docsForWord);
                }
            }

            return result ?? new HashSet<int>();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== САМОСТІЙНА РОБОТА №9: Dictionary<TKey, TValue> та HashSet<T> ===\n");

            Console.WriteLine("--- 1. Кеш метаданих файлів (FileMetadataCache) ---");
            var fileCache = new FileMetadataCache();
            fileCache.Add("/docs/report.pdf", new FileMetadata
            {
                FileName = "report.pdf",
                Size = 245000,
                LastModified = DateTime.Now.AddHours(-2),
                Hash = "A1B2C3D4"
            });
            fileCache.Add("/src/Program.cs", new FileMetadata
            {
                FileName = "Program.cs",
                Size = 4820,
                LastModified = DateTime.Now,
                Hash = "E5F6G7H8"
            });

            Console.WriteLine($"Пошук '/DOCS/REPORT.PDF' (OrdinalIgnoreCase): {fileCache.Get("/DOCS/REPORT.PDF")}");
            Console.WriteLine($"Чи є '/src/Program.cs': {fileCache.Contains("/src/Program.cs")}");
            fileCache.Remove("/src/Program.cs");
            Console.WriteLine($"Кількість у кеші після видалення: {fileCache.Count}\n");

            Console.WriteLine("--- 2. Менеджер унікальних тегів (TagManager) ---");
            var tagManager = new TagManager();
            tagManager.AddTag("post-1", "CSharp");
            tagManager.AddTag("post-1", "DOTNET");
            tagManager.AddTag("post-1", "csharp"); 
            tagManager.AddTag("post-1", "LINQ");

            tagManager.AddTag("post-2", "dotnet");
            tagManager.AddTag("post-2", "LINQ");
            tagManager.AddTag("post-2", "Performance");

            Console.WriteLine($"Усі унікальні теги (UnionWith): {string.Join(", ", tagManager.GetAllUniqueTags())}");
            Console.WriteLine($"Спільні теги для post-1 та post-2 (IntersectWith): {string.Join(", ", tagManager.FindCommonTags("post-1", "post-2"))}\n");

            Console.WriteLine("--- 3. Інвертований пошуковий індекс (SearchIndex) ---");
            var searchIndex = new SearchIndex();
            searchIndex.AddDocument(1, "C# та .NET дозволяють створювати швидкі веб-застосунки.");
            searchIndex.AddDocument(2, "Колекції Dictionary та HashSet у .NET мають складність O(1).");
            searchIndex.AddDocument(3, "C# використовує HashSet для зберігання унікальних елементів.");

            var singleWordDocs = searchIndex.Search("HashSet");
            Console.WriteLine($"Документи зі словом 'HashSet': [{string.Join(", ", singleWordDocs)}]");

            var multiWordDocs = searchIndex.SearchMultiple("C#", "HashSet");
            Console.WriteLine($"Документи зі словами 'C#' ТА 'HashSet': [{string.Join(", ", multiWordDocs)}]");
        }
    }
}