using System;

namespace UnifiedModel.SourceGenerator.Helpers
{
    public static class Logger
    {
        public static bool AllowLogs { get; set; }

        public static void Log(string message, bool overrideAllowLogs = false)
        {
            if (AllowLogs || overrideAllowLogs)
            {
                Console.WriteLine($"{DateTime.Now} - {message}");
            }
        }
    }
}