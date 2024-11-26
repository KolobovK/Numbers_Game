using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// Класс игрока в котором будет обрабатываться пользователь
    /// </summary>
    internal class Player
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Количество ходов в игре
        /// </summary>
        public int Moves { get; set; }

        //static string filePath = "players.txt";
        /// <summary>
        /// Конструктор создания пользователя
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="moves">Количество ходов</param>
        public Player(string name, string password, int moves)
        {
            Name = name;
            Password = password;
            Moves = moves;
        }

        #region Authentication
        /// <summary>
        /// Аутентификация пользователя по логину и паролю.
        /// </summary>
        /// <returns>true, если аутентификация прошла успешно; иначе false.</returns>
        public static bool AuthenticateUser(List<Player> players, out string playerName)
        {
            Logger.LogEvent("Попытка аутентификации.");
            Console.Write("Введите ваш логин: ");
            string _name = Console.ReadLine();

            Console.Write("Введите ваш пароль: ");
            string _password = Console.ReadLine();

            var _player = players.FirstOrDefault(p => p.Name == _name && p.Password == _password);

            if (_player == null)
            {
                Console.WriteLine("Неверный логин или пароль.");
                Logger.LogEvent("Ошибка аутентификации: неверный логин или пароль.");
                playerName = String.Empty;
                return false;
            }

            playerName = _name;// Устанавливаем имя текущего игрока
            Logger.LogEvent("Успешная аутентификация.", playerName);
            return true;
        }

        /// <summary>
        /// Регистрация нового пользователя с заданным логином и паролем.
        /// </summary>
        /// <remarks>
        /// Запрашивает логин и пароль у пользователя. Если логин уже занят, выводит сообщение об ошибке.
        /// Если регистрация прошла успешно, новый игрок добавляется в список и сохраняется в файл.
        /// </remarks>
        public static void RegisterUser(List<Player> players)
        {
            Logger.LogEvent("Попытка регистрации нового пользователя.");
            Console.Write("Введите ваш логин: ");
            string _name = Console.ReadLine();

            Console.Write("Введите ваш пароль: ");
            string _password = Console.ReadLine();

            if (players.Any(p => p.Name == _name))
            {
                Console.WriteLine("Игрок с таким логином уже зарегистрирован.");
                Logger.LogEvent("Ошибка: игрок с таким логином уже зарегистрирован.");
                return;
            }

            var _newPlayer = new Player(_name, _password, 0);
            players.Add(_newPlayer);
            SavePlayer(_name, _password, 0);

            Console.WriteLine("Регистрация успешна.");
            Logger.LogEvent("Регистрация успешна.", _name);
        }
        #endregion
        /// <summary>
        /// Сохраняет данные игрока в файл.
        /// </summary>
        /// <param name="playerName">Имя игрока, которому принадлежат данные.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <param name="moves">Количество ходов, выполненных игроком.</param>
        static void SavePlayer(string playerName, string password, int moves)
        {
            using (StreamWriter _sw = new StreamWriter(LogicGame.FilePath, true))
            {
                _sw.WriteLine($"{playerName},{password},{moves}");
            }
        }
    }
}
