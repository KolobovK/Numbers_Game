using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// Класс для логирования событий
    /// </summary>
    public class Logger
    {
        private static string logFilePath = "game_log.txt"; // Путь к лог-файлу

        /// <summary>
        /// Метод для логирования событий
        /// </summary>
        /// <param name="message">Событие</param>
        /// <param name="playerName">Имя игрока</param>
        public static void LogEvent(string message, string playerName = null)
        {
            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = playerName != null
                ? $"{timeStamp} - Игрок: {playerName} - {message}"
                : $"{timeStamp} - {message}";

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logEntry);
            }
        }
    }
}
