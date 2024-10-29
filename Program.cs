using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class Program
    {
        static List<Player> players = new List<Player>();
        static string filePath = "players.txt";
        static string playerName;
        static void Main(string[] args)
        {
            LoadPlayers();

            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Войти");
                Console.WriteLine("2. Зарегистрироваться");
                Console.WriteLine("3. Выход");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    if (AuthenticateUser())
                    {
                        StartGame();
                    }
                }
                else if (choice == "2")
                {
                    RegisterUser();
                }
                else if (choice == "3")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                }
            }       
        }
        static void StartGame()
        {
            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Новая игра");
                Console.WriteLine("2. Таблица игроков");
                Console.WriteLine("3. Выход");

                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    int moves = PlayGame();
                    double percentage = CalculatePlayerPercentage(moves);
                    SavePlayerProgress(playerName, moves);
                    Console.WriteLine($"Вы находитесь в {percentage:F2}% всех игроков.");
                    Console.WriteLine();
                }
                else if (choice == "2")
                {
                    if (players.Count == 0)
                    {
                        Console.WriteLine("Таблица пуста");
                    }
                    else
                        ShowLeaderboard();
                }
                else if (choice == "3")
                {
                    break;
                }
            }
        }

        static void SavePlayerProgress(string playerName, int moves)
        {
            var player = players.First(p => p.Name == playerName);
            if (player.Moves == 0 || moves < player.Moves)
            {
                player.Moves = moves;

                // Перезаписываем файл с обновленными данными
                File.WriteAllLines(filePath, players.Select(p => $"{p.Name},{p.Password},{p.Moves}"));
            }
        }

        static void LoadPlayers()
        {
            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var parts = line.Split(',');
                    if (parts.Length == 3)
                    {
                        players.Add(new Player(parts[0], parts[1], Convert.ToInt32(parts[2])));
                    }
                }
            }
        }

        static void SavePlayer(string playerName, string password, int moves)
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine($"{playerName},{password},{moves}");
            }
        }

        static bool AuthenticateUser()
        {
            Console.Write("Введите ваш логин: ");
            string name = Console.ReadLine();

            Console.Write("Введите ваш пароль: ");
            string password = Console.ReadLine();

            var player = players.FirstOrDefault(p => p.Name == name && p.Password == password);

            if (player == null)
            {
                Console.WriteLine("Неверный логин или пароль.");
                return false;
            }

            playerName = name;  // Устанавливаем имя текущего игрока
            return true;
        }

        static void RegisterUser()
        {
            Console.Write("Введите ваш логин: ");
            string name = Console.ReadLine();

            Console.Write("Введите ваш пароль: ");
            string password = Console.ReadLine();

            if (players.Any(p => p.Name == name))
            {
                Console.WriteLine("Игрок с таким логином уже зарегистрирован.");
                return;
            }

            var newPlayer = new Player(name, password, 0);
            players.Add(newPlayer);
            SavePlayer(name, password, 0);

            Console.WriteLine("Регистрация успешна.");
        }

        static int PlayGame()
        {
            int generate = GenerateUniqueDigitNumber();
            string x = generate.ToString();
            Console.WriteLine(x);
            int position = 0;
            int digit = 0;
            int moves = 0;

            while (true)
            {
                string ot;
                while (true)
                {
                    Console.Write("Введите 4-значное число: ");
                    ot = Console.ReadLine();
                    if (ot.Length == 4 && int.TryParse(ot, out _))
                        break;
                    Console.WriteLine("Ошибка: введите ровно 4 цифры.");
                }

                moves++;
                for (int i = 0; i < 4; i++)
                {
                    if (x[i] == ot[i])
                        position++;
                    if (x.Contains(ot[i]) && x[i] != ot[i])
                        digit++;
                }

                Console.WriteLine($"{position} на месте, всего {digit + position}.");

                if (position == 4)
                {
                    Console.WriteLine("Поздравляю! Вы угадали число!");
                    break;
                }
                position = 0;
                digit = 0;
            }
            Console.WriteLine($"Загаданное число: {x}");
            return moves;
        }

        static void ShowLeaderboard()
        {
            var rankedPlayers = players
                .OrderBy(p => p.Moves == 0 ? int.MaxValue : p.Moves) // Сначала сортируем по количеству ходов
                .ThenBy(p => p.Name) // Затем по имени
                .GroupBy(p => p.Moves) // Группируем по количеству ходов
                .SelectMany(g => g.Select(player => new { Player = player, Place = g.Key })) // Создаем 'слой' для мест
                .Distinct() // Убираем дубликаты
                .OrderBy(p => p.Place) // Упорядочиваем по количеству ходов для печати
                .ToList();

            int displayedPlace = 1;
            foreach (var group in rankedPlayers.GroupBy(x => x.Place))
            {
                foreach (var player in group)
                {
                    Console.WriteLine($"Место: {displayedPlace}, Игрок: {player.Player.Name}, Ходов: {player.Player.Moves}");
                }
                displayedPlace++; // Увеличиваем место только на 1 для каждой группы
            }
            Console.WriteLine();
        }
        static int GenerateUniqueDigitNumber()
        {
            Random rnd = new Random();
            string number = "";
            while (number.Length < 4)
            {
                int digit = rnd.Next(0, 10);
                if (!number.Contains(digit.ToString())) // Проверяем на уникальность
                {
                    number += digit;
                }
            }
            return int.Parse(number);
        }
        static double CalculatePlayerPercentage(int moves)
        {
            players.OrderBy(player => player.Moves);
            int placement = players.FindIndex(player => player.Moves == moves) + 2;
            int totalPlayers = players.Count;
            if (totalPlayers == 0)
                return 100; // Если никого нет, игрок попадает в 100%
            return placement * 100.0 / totalPlayers;
        }
    }
    class Player
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int Moves { get; set; }

        public Player(string name, string password, int moves)
        {
            Name = name;
            Password = password;
            Moves = moves;
        }
    }
}
