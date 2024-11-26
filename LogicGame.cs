using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// Класс в котором происходит логика игры
    /// </summary>
    internal class LogicGame
    {
        /// <summary>
        /// Список игроков полученный из файла
        /// </summary>
        static List<Player> players = new List<Player>();
        /// <summary>
        /// Адрес файла игроков
        /// </summary>
        public static string FilePath = "players.txt";
        /// <summary>
        /// Имя игрока, который зашёл в игру
        /// </summary>
        static string playerName = string.Empty;

        /// <summary>
        /// Запуск начального меню
        /// </summary>
        public static void StartMenu()
        {
            Logger.LogEvent("Запуск меню");
            LoadPlayers();

            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Войти");
                Console.WriteLine("2. Зарегистрироваться");
                Console.WriteLine("3. Выход");

                string _choice = Console.ReadLine();

                if (_choice == "1")
                {
                    if (Player.AuthenticateUser(players, out playerName))
                    {
                        Logger.LogEvent("Успешный вход.", playerName);
                        StartGame();
                    }
                    else
                    {
                        Logger.LogEvent("Неудачный вход."); 
                    }
                }
                else if (_choice == "2")
                {
                    Logger.LogEvent("Попытка регистрации.");
                    Player.RegisterUser(players);
                }
                else if (_choice == "3")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                    Logger.LogEvent("Неверный выбор на главном меню.");
                }
            }
        }
        /// <summary>
        /// Запуск игры после авторизации
        /// </summary>
        public static void StartGame()
        {
            Logger.LogEvent("Запуск игрового процесса.");
            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Новая игра");
                Console.WriteLine("2. Таблица игроков");
                Console.WriteLine("3. Выход");

                string _choice = Console.ReadLine();
                if (_choice == "1")
                {
                    // Запускает новую игру и сохраняет прогресс игрока
                    Logger.LogEvent("Начало новой игры.", playerName);
                    int _moves = PlayGame();
                    double _percentage = CalculatePlayerPercentage(_moves);
                    SavePlayerProgress(_moves);
                    Console.WriteLine($"Вы находитесь в {_percentage:F2}% всех игроков.");
                    Logger.LogEvent($"Игра завершена. Ходы: {_moves}", playerName);
                    Console.WriteLine();
                }
                else if (_choice == "2")
                {
                    Logger.LogEvent("Просмотр таблицы игроков.", playerName);
                    // Показывает таблицу игроков или сообщение о пустой таблицы
                    if (players.Count == 0)
                    {
                        Console.WriteLine("Таблица пуста");
                    }
                    else
                        ShowLeaderboard(players);
                }
                else if (_choice == "3")
                {
                    // Прерывает цикл и завершает игру
                    Logger.LogEvent("Выход из игры.");
                    break;
                }
                else
                {
                    Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                    Logger.LogEvent("Неверный выбор в игровом меню.", playerName); 
                }
            }
        }

        /// <summary>
        /// Запуск игры, где игроку необходимо угадать 4-значное число.
        /// </summary>
        /// <returns>Количество ходов, выполненных игроком до угадывания числа.</returns>
        public static int PlayGame()
        {
            int _generate = GenerateUniqueDigitNumber();
            string _x = _generate.ToString();
            Console.WriteLine(_x);
            int _position = 0;
            int _digit = 0;
            int _moves = 0;

            while (true)
            {
                string _answer;
                while (true)
                {
                    Console.Write("Введите 4-значное число: ");
                    _answer = Console.ReadLine();
                    if (_answer.Length == 4 && int.TryParse(_answer, out _))
                        break;
                    Console.WriteLine("Ошибка: введите ровно 4 цифры.");
                    Logger.LogEvent($"Ошибка ввода: {_answer} не корректно.", playerName);
                }

                _moves++;
                for (int i = 0; i < 4; i++)
                {
                    if (_x[i] == _answer[i])
                        _position++;
                    if (_x.Contains(_answer[i]) && _x[i] != _answer[i])
                        _digit++;
                }

                Console.WriteLine($"{_position} на месте, всего {_digit + _position}.");
                Logger.LogEvent($"Ввод: {_answer}. Результат: {_position} на месте, всего {_digit + _position}.", playerName);

                if (_position == 4)
                {
                    Console.WriteLine("Поздравляю! Вы угадали число!");
                    Logger.LogEvent("Игрок угадал число!", playerName);
                    break;
                }
                _position = 0;
                _digit = 0;
            }
            Console.WriteLine($"Загаданное число: {_x}");
            return _moves;
        }

        /// <summary>
        /// Генерирует уникальное 4-значное число.
        /// </summary>
        /// <returns>Сгенерированное 4-значное число.</returns>
        static int GenerateUniqueDigitNumber()
        {
            Random _rnd = new Random();
            string _number = "";
            Logger.LogEvent("Генерация уникального 4-значного числа.");
            while (_number.Length < 4)
            {
                int _digit = _rnd.Next(1, 10);
                if (!_number.Contains(_digit.ToString())) // Проверяем на уникальность
                {
                    _number += _digit;
                }
            }
            Logger.LogEvent($"Сгенерированное число: {_number}");
            return int.Parse(_number);
        }

        /// <summary>
        /// Отображает таблицу лидеров с рейтингом игроков.
        /// </summary>
        public static void ShowLeaderboard(List<Player> players)
        {
            Logger.LogEvent("Отображение таблицы лидеров.");

            var _rankedPlayers = players
                .OrderBy(p => p.Moves == 0 ? int.MaxValue : p.Moves) // Сначала сортируем по количеству ходов
                .ThenBy(p => p.Name) // Затем по имени
                .GroupBy(p => p.Moves) // Группируем по количеству ходов
                .SelectMany(g => g.Select(player => new { Player = player, Place = g.Key })) // Создаем 'слой' для мест
                .Distinct() // Убираем дубликаты
                .OrderBy(p => p.Place) // Упорядочиваем по количеству ходов для печати
                .ToList();

            int _displayedPlace = 1;
            foreach (var _group in _rankedPlayers.GroupBy(x => x.Place))
            {
                foreach (var _player in _group)
                {
                    Console.WriteLine($"Место: {_displayedPlace}, Игрок: {_player.Player.Name}, Ходов: {_player.Player.Moves}");
                }
                _displayedPlace++; // Увеличиваем место только на 1 для каждой группы
            }
            Logger.LogEvent("Таблица лидеров успешно отображена.");
            Console.WriteLine();
        }

        /// <summary>
        /// Вычисляет процентное соотношение игрока на основе количества сделанных ходов.
        /// </summary>
        /// <param name="moves">Количество ходов, сделанных игроком.</param>
        /// <returns>Процентное соотношение игрока среди всех игроков.</returns>
        static double CalculatePlayerPercentage(int moves)
        {
            Logger.LogEvent($"Вычисление процентного соотношения для {moves} ходов.");

            players.OrderBy(player => player.Moves);
            int _placement = players.FindIndex(player => player.Moves == moves) + 2;
            int _totalPlayers = players.Count;
            if (_totalPlayers == 0)
            {
                Logger.LogEvent("Нет игроков для вычисления процентного соотношения. Возвращаем 100.");
                return 100; // Если никого нет, игрок попадает в 100%
            }
            double _percentage = _placement * 100.0 / _totalPlayers;
            Logger.LogEvent($"Процентное соотношение вычислено: {_percentage:F2}%"); 
            return _percentage;
        }

        #region WorkFile
        /// <summary>
        /// Сохраняет прогресс игрока в файл
        /// </summary>
        /// <param name="playerName">Имя игрока, чьи данные необходимо сохранить.</param>
        /// <param name="moves">Количество ходов, совершенных игроком в текущей игре.</param>
        static void SavePlayerProgress(int moves)
        {
            Logger.LogEvent($"Сохранение прогресса игрока {playerName} с {moves} ходами.");
            var _player = players.First(p => p.Name == playerName);
            if (_player.Moves == 0 || moves < _player.Moves)
            {
                _player.Moves = moves;
                // Перезаписываем файл с обновленными данными
                File.WriteAllLines(FilePath, players.Select(p => $"{p.Name},{p.Password},{p.Moves}"));
                Logger.LogEvent($"Прогресс игрока {playerName} успешно сохранен.");
            }
        }
        /// <summary>
        /// Загрузка игроков из файла в список игроков.
        /// </summary>
        static void LoadPlayers()
        {
            Logger.LogEvent("Загрузка игроков из файла.");
            //TODO: При выгрузке игроков проверять, что они не повторяются
            try
            {
                if (File.Exists(FilePath))
                {
                    foreach (var _line in File.ReadAllLines(FilePath))
                    {
                        var _parts = _line.Split(',');
                        if (_parts.Length == 3)
                        {
                            players.Add(new Player(_parts[0], _parts[1], Convert.ToInt32(_parts[2])));
                        }
                    }
                    Logger.LogEvent($"Загружено игроков: {players.Count}.");
                }
                else
                {
                    Logger.LogEvent("Файл не найден. Игроки не загружены.");
                }
            }
            catch (IOException ex) // Обработка ошибок ввода-вывода
            {
                Console.WriteLine("Ошибка при загрузке данных игроков: " + ex.Message);
                Logger.LogEvent("Ошибка при загрузке данных игроков: " + ex.Message);
            }
            catch (FormatException ex) // Обработка ошибок формата чисел
            {
                Console.WriteLine("Ошибка формата данных в файле: " + ex.Message);
                Logger.LogEvent("Ошибка формата данных в файле: " + ex.Message);
            }
            catch (Exception ex) // Общая обработка других исключений
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
                Logger.LogEvent("Произошла ошибка: " + ex.Message);
            }
        }
        #endregion
    }
}
