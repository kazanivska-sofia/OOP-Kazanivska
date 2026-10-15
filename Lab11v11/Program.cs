using System;
using System.Collections.Generic;

namespace Lab11v11
{
    public abstract class DataExporter
    {
        public string SourceName { get; set; }
        public bool IsCompressed { get; set; }

        public DataExporter(string sourceName)
        {
            SourceName = sourceName;
            IsCompressed = false;
        }

        public void Export()
        {
            Console.WriteLine($"=== Початок експорту даних з '{SourceName}' ===");
            FetchData();
            Transform();
            Format();
            
            // Віртуальний крок
            if (ShouldCompress())
            {
                Compress();
            }

            Save();
            CloseConnection();
            Console.WriteLine("=== Експорт успішно завершено ===\n");
        }

        protected abstract void FetchData();
        protected abstract void Format();
        protected abstract void Save();

        protected virtual void Transform()
        {
            Console.WriteLine("[Transform] Стандартне очищення та нормалізація даних...");
        }

        protected virtual bool ShouldCompress()
        {
            return IsCompressed;
        }

        protected virtual void Compress()
        {
            Console.WriteLine("[Compress] Стиснення даних у ZIP-архів...");
        }

        protected virtual void CloseConnection()
        {
            Console.WriteLine("[CloseConnection] З'єднання з джерелом даних закрито.");
        }
    }

    public class CSVExporter : DataExporter
    {
        public CSVExporter(string sourceName) : base(sourceName) { }

        protected override void FetchData()
        {
            Console.WriteLine("[FetchData] Отримання табличних даних із SQL-бази...");
        }

        protected override void Format()
        {
            Console.WriteLine("[Format] Форматування даних у формат CSV (значення через кому)...");
        }

        protected override void Save()
        {
            Console.WriteLine("[Save] Збереження файлу 'export_data.csv' на диск.");
        }
    }

    public class JSONExporter : DataExporter
    {
        public JSONExporter(string sourceName, bool enableCompression) : base(sourceName)
        {
            IsCompressed = enableCompression;
        }

        protected override void FetchData()
        {
            Console.WriteLine("[FetchData] Отримання об'єктів з API сервісу...");
        }

        protected override void Transform()
        {
            base.Transform();
            Console.WriteLine("[Transform] Додаткове сереалізаційне перетворення ключів у camelCase...");
        }

        protected override void Format()
        {
            Console.WriteLine("[Format] Форматування даних у структуру JSON із відступами...");
        }

        protected override void Save()
        {
            Console.WriteLine("[Save] Відправка JSON-струменя у хмарне сховище.");
        }
    }

    public class XMLExporter : DataExporter
    {
        public XMLExporter(string sourceName) : base(sourceName) 
        {
            IsCompressed = true; // За замовчуванням стискаємо XML
        }

        protected override void FetchData()
        {
            Console.WriteLine("[FetchData] Зчитування документа з локальної бази...");
        }

        protected override void Format()
        {
            Console.WriteLine("[Format] Генерація XML-дерева з тегами та атрибутами...");
        }

        protected override void Save()
        {
            Console.WriteLine("[Save] Запис файлу 'export_data.xml' на сервер.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<DataExporter> exporters = new List<DataExporter>
            {
                new CSVExporter("UsersDB"),
                new JSONExporter("AnalyticsAPI", enableCompression: true),
                new XMLExporter("LegacySystem")
            };

            foreach (var exporter in exporters)
            {
                exporter.Export();
            }
        }
    }
}