
using System.Text;

namespace ConfigurationService
{
    public static class Logger
    {
        //Путь к файлу логов
        private static string _pathToFileLog = @"Log.txt";

        // Статическове событие к которому можно подписаться
        private static event Action<string> _eventLog;

        // метод для вызова событий логирования
        public static void Log(string message)
        {
            _eventLog?.Invoke($"[EVENT] {DateTime.Now} : {message}{Environment.NewLine}");
        }

        // Метод обработчик события (Запись в файл)
        private static async void WriteToFile(string message)
        {
            try
            {

                if(File.Exists(_pathToFileLog))
                {
                    message += File.ReadAllText(_pathToFileLog);
                }
                else
                {
                    Console.WriteLine($"Не удалось прочитать {_pathToFileLog}");
                }

                File.WriteAllText(_pathToFileLog, message);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Ошибка записи в Log файл {ex.Message}");
            }
        }

        public static string ReadLogFile()
        {
            try
            {
                return File.ReadAllText(_pathToFileLog, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения Log файла {ex.Message}");
                return string.Empty;
            }
        }

        // Статический конструктор для инициализации событий логирования по умолчанию
        static Logger()
        {
            _eventLog += WriteToFile;
            _eventLog += Console.WriteLine;
        }
    }
}
