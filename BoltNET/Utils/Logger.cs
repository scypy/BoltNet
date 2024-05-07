using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BoltNET.Utils
{
    public static class Logger
    {
        private static string _logFilePath = AppDomain.CurrentDomain.BaseDirectory;
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
        public static void LogException(this Exception ex)
        {
            if (!Environment.HasShutdownStarted) Log(ex.ToString());
            try
            {
                using (StreamWriter writer = new StreamWriter(_logFilePath, false))
                {
                    writer.WriteLine($"Timestamp: {DateTime.Now}");
                    writer.WriteLine($"Exception Type: {ex.GetType().FullName}");
                    writer.WriteLine($"Message: {ex.Message}");
                    writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                    writer.WriteLine(new string('-', 50));
                }
            }
            catch (Exception logEx)
            {
                // If an exception occurs while logging, output it to the console
                $"Error occurred while logging exception: {logEx.Message}".Log();
            }
        }
    }
}
