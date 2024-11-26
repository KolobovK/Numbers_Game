using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    /*
     * В данной программе компьютер загадывает четырёхзначное число и игрок пытается угадать, что это за число.
     * Пользователю необходимо зарегистрироваться для начала игры,
     * после этого он может насладиться игрой или посмотреть таблицу лидеров.
     */
    internal class Program
    {
        static void Main(string[] args)
        {
            LogicGame.StartMenu();
        }
    }
}
