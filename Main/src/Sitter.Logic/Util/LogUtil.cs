using System;
using MySitterHub.Model.Misc;

namespace MySitterHub.Logic.Util
{
    public class LogUtil
    {
        private AppLogger _appLogger = new AppLogger();

        public void LogMessage(string message)
        {
            LogMessage(message, ConsoleColor.White, true);
        }

        public void LogMessage(string message, ConsoleColor color, bool lineBreak = true)
        {
            Console.ForegroundColor = color;

            if (lineBreak)
                Console.WriteLine(message);
            else
                Console.Write(message);

            Console.ResetColor();

            _appLogger.Info(null, message);
        }

    }
}