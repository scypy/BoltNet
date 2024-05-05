using System;
using System.Collections.Generic;
using System.Text;

namespace BoltNET.Utils
{
    public static class Logger
    {
        public static void Log(this string str, ConsoleColor color = ConsoleColor.White)
        {
            lock (Console.Out)
            {
                str = $"[{DateTime.Now:G}]:{str}";
                if (Console.ForegroundColor != color)
                    Console.ForegroundColor = color;
                else Console.WriteLine(str);
                if (Console.ForegroundColor != color)
                    Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
