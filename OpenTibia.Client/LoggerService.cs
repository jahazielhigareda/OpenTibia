using System;
using OpenTibia.Game.Common.ServerObjects;

namespace OpenTibia.Client
{
    public static class LoggerService
    {
        private static ILogger _logger;

        static LoggerService()
        {
            // Default initialization
            _logger = new Logger(new ConsoleLoggerProvider(), LogLevel.Debug);
        }

        public static void Setup(ILogger logger)
        {
            _logger = logger;
        }

        public static void Debug(string message) => _logger.WriteLine($"[DEBUG] {message}", LogLevel.Debug);
        public static void Info(string message)  => _logger.WriteLine($"[INFO]  {message}", LogLevel.Information);
        public static void Warn(string message)  => _logger.WriteLine($"[WARN]  {message}", LogLevel.Warning);
        public static void Error(string message) => _logger.WriteLine($"[ERROR] {message}", LogLevel.Error);

        // Layer-specific helpers
        public static void UI(string message)   => _logger.WriteLine($"[UI]  {message}", LogLevel.Information);
        public static void Net(string message)  => _logger.WriteLine($"[NET] {message}", LogLevel.Debug);
        public static void Srv(string message)  => _logger.WriteLine($"[SRV] {message}", LogLevel.Information);
        public static void Cmd(string message)  => _logger.WriteLine($"[CMD] {message}", LogLevel.Information);
    }
}
