using System;
using System.Collections.Generic;

namespace Lab10v11
{
    public interface ICustomComparer<T>
    {
        int Compare(T x, T y);
    }

    public class StringLengthComparer : ICustomComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            return x.Length.CompareTo(y.Length);
        }
    }

    public class AlphabeticalComparer : ICustomComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }

    public abstract class FileProcessor
    {
        public string FileName { get; protected set; } = string.Empty;
        public string Content { get; protected set; } = string.Empty;

        public abstract void ReadFile(string path);
        public abstract void ProcessContent();

        public void WriteResult(string path)
        {
            Console.WriteLine($"[FileProcessor] Результат збережено у файл '{path}'. Зміст: \"{Content}\"");
        }
    }

    public class LogFileProcessor : FileProcessor
    {
        public override void ReadFile(string path)
        {
            FileName = path;
            Content = "2026-10-08 15:30:00 [INFO] System initialized successfully.";
            Console.WriteLine($"[LogFileProcessor] Зчитано лог-файл '{FileName}'");
        }

        public override void ProcessContent()
        {
            Content = Content.ToUpper();
            Console.WriteLine("[LogFileProcessor] Зміст лог-файла переведено у верхній регістр.");
        }
    }

    public class ConfigFileProcessor : FileProcessor
    {
        public override void ReadFile(string path)
        {
            FileName = path;
            Content = "DbConnectionString=Server=localhost;Database=TestDb;";
            Console.WriteLine($"[ConfigFileProcessor] Зчитано конфіг '{FileName}'");
        }

        public override void ProcessContent()
        {
            Content = Content.Replace("localhost", "192.168.1.100");
            Console.WriteLine("[ConfigFileProcessor] Адресу сервера в конфігурації оновлено.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. Демонстрація роботи Інтерфейсу (ICustomComparer) ===\n");

            List<ICustomComparer<string>> comparers = new List<ICustomComparer<string>>
            {
                new StringLengthComparer(),
                new AlphabeticalComparer()
            };

            string str1 = "Apple";
            string str2 = "Watermelon";

            foreach (var comparer in comparers)
            {
                int result = comparer.Compare(str1, str2);
                string comparerType = comparer.GetType().Name;
                Console.WriteLine($"[{comparerType}] Порівняння '{str1}' та '{str2}': Результат = {result}");
            }

            Console.WriteLine("\n=== 2. Демонстрація роботи Абстрактного класу (FileProcessor) ===\n");

            List<FileProcessor> processors = new List<FileProcessor>
            {
                new LogFileProcessor(),
                new ConfigFileProcessor()
            };

            foreach (var processor in processors)
            {
                processor.ReadFile("sample_file.txt");
                processor.ProcessContent();
                processor.WriteResult("output_file.txt");
                Console.WriteLine(new string('-', 50));
            }
        }
    }
}