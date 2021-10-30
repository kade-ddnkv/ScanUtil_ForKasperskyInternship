using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtil_ForKasperskyInternship
{
    /// <summary>
    /// Класс для вывода различных ошибок.
    /// </summary>
    class ErrorMessages
    {
        /// <summary>
        /// Сообщает о некорректном вводе и выводит правила запуска утилиты.
        /// </summary>
        public static void PrintIncorrectInput()
        {
            Console.WriteLine("Incorrect input!" + Environment.NewLine +
                "For creating new scan task, type:" + Environment.NewLine +
                @"  scan_util scan %userprofile%\Documents" + Environment.NewLine +
                "For checking task status, type:" + Environment.NewLine +
                @"  scan_util status 1234");
        }

        public static void PrintIncorrectDirectory()
        {
            Console.WriteLine("Error! Entered directory doesn't exist or is blocked.\n" +
                "Try to select different directory.");
        }

        public static void PrintIncorrectTaskNumber()
        {
            Console.WriteLine("Error! After a \"status\" should be a non-negative number.");
        }

        public static void PrintServerIsNotRunning()
        {
            Console.WriteLine("Server doesn't answer. Start scan_service first!");
        }
    }
}
