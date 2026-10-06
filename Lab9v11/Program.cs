using System;
using System.Collections.Generic;
using System.IO;

namespace Lab9v11
{
    public abstract class DataImporter
    {
        public string ImporterName { get; set; }

        public DataImporter(string importerName)
        {
            ImporterName = importerName;
        }

        // Абстрактний метод для імпорту даних
        public abstract List<string> Import(string filePath);
    }

    public class ExcelImporter : DataImporter
    {
        public ExcelImporter() : base("Excel Importer") { }

        public override List<string> Import(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Шлях до файла не може бути порожнім!");

            if (!filePath.EndsWith(".xlsx") && !filePath.EndsWith(".xls"))
                throw new FormatException($"Файл '{filePath}' має невалідний формат для Excel (очікується .xlsx або .xls).");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл Excel за шляхом '{filePath}' не знайдено!");

            Console.WriteLine($"[Excel] Успішно імпортовано дані з {filePath}");
            return new List<string> { "Excel_Row1", "Excel_Row2", "Excel_Row3" };
        }
    }

    public class CSVImporter : DataImporter
    {
        public CSVImporter() : base("CSV Importer") { }

        public override List<string> Import(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Шлях до файла не може бути порожнім!");

            if (!filePath.EndsWith(".csv"))
                throw new FormatException($"Файл '{filePath}' має невалідний формат для CSV (очікується .csv).");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл CSV за шляхом '{filePath}' не знайдено!");

            Console.WriteLine($"[CSV] Успішно імпортовано дані з {filePath}");
            return new List<string> { "CSV_Row1", "CSV_Row2" };
        }
    }

    // 3. Імпортер JSON
    public class JSONImporter : DataImporter
    {
        public JSONImporter() : base("JSON Importer") { }

        public override List<string> Import(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Шлях до файла не може бути порожнім!");

            if (!filePath.EndsWith(".json"))
                throw new FormatException($"Файл '{filePath}' має невалідний формат для JSON (очікується .json).");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл JSON за шляхом '{filePath}' не знайдено!");

            Console.WriteLine($"[JSON] Успішно імпортовано дані з {filePath}");
            return new List<string> { "JSON_Object1", "JSON_Object2" };
        }
    }

    public class ImportService
    {
        public void ProcessAllImports(List<(DataImporter Importer, string FilePath)> importTasks)
        {
            Console.WriteLine("=== Запуск процесу імпорту даних ===\n");

            foreach (var task in importTasks)
            {
                try
                {
                    Console.WriteLine($"Спроба імпорту через: {task.Importer.ImporterName}");
                    List<string> data = task.Importer.Import(task.FilePath);
                    Console.WriteLine($"Отримано записів: {data.Count}\n");
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"[ПОМИЛКА КЛІЄНТА/ФАЙЛА] {ex.Message}\n");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"[ПОМИЛКА ФОРМАТУ] {ex.Message}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[НЕПЕРЕДБАЧЕНА ПОМИЛКА] {ex.Message}\n");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            File.WriteAllText("data.csv", "col1,col2");
            File.WriteAllText("data.json", "{ }");

            ImportService service = new ImportService();

            var tasks = new List<(DataImporter Importer, string FilePath)>
            {
                (new CSVImporter(), "data.csv"),                 
                (new JSONImporter(), "data.json"),               
                (new ExcelImporter(), "wrong_file.txt"),         
                (new ExcelImporter(), "missing_data.xlsx")      
            };

            service.ProcessAllImports(tasks);

            if (File.Exists("data.csv")) File.Delete("data.csv");
            if (File.Exists("data.json")) File.Delete("data.json");
        }
    }
}