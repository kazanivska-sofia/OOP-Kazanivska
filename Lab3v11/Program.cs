using System;

namespace Lab3v11
{
    public class AudioPlayer : IDisposable
    {
        private string _audioFilePath;
        private bool _isPlaying;
        private bool _disposed = false;

        public AudioPlayer(string filePath)
        {
            _audioFilePath = filePath;
            _isPlaying = true;
            Console.WriteLine($"[AudioPlayer] Файл '{_audioFilePath}' відкрито. Запуск відтворення...");
        }

        public bool IsPlaying => _isPlaying;
        public string AudioFilePath => _audioFilePath;

        public void Play()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(AudioPlayer), "Неможливо відтворити: плеєр закритий/звільнений!");
            }
            Console.WriteLine($"[AudioPlayer] Відтворення файлу '{_audioFilePath}' триває...");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine($"[Dispose(true)] Зупинка відтворення файлу '{_audioFilePath}' та закриття файлового потоку...");
                }

                if (_isPlaying)
                {
                    Console.WriteLine($"[Dispose] Звільнення аудіо-буфера та системних ресурсів звукової карти.");
                    _isPlaying = false;
                }

                _disposed = true;
            }
        }

        ~AudioPlayer()
        {
            Console.WriteLine($"[~AudioPlayer] Викликано деструктор для '{_audioFilePath}'.");
            Dispose(false);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Сценарій 1: Використання конструкції using ===");
            using (AudioPlayer player1 = new AudioPlayer("song_rock.mp3"))
            {
                player1.Play();
            }
            Console.WriteLine("Сценарій 1 завершено.\n");

            Console.WriteLine("=== Сценарій 2: Явний виклик Dispose() без using ===");
            AudioPlayer player2 = new AudioPlayer("podcast_ep1.wav");
            player2.Play();
            player2.Dispose();
            Console.WriteLine("Сценарій 2 завершено.\n");

            Console.WriteLine("=== Сценарій 3: Демонстрація роботи деструктора (GC.Collect) ===");
            CreateUnreferencedPlayer();
            
            Console.WriteLine("Запуск GC.Collect()...");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.WriteLine("Сценарій 3 завершено.");
        }

        static void CreateUnreferencedPlayer()
        {
            AudioPlayer player3 = new AudioPlayer("track_background.flac");
            player3.Play();
        }
    }
}