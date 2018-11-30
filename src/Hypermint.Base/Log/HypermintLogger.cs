using Microsoft.Extensions.Logging;
using Prism.Logging;
using System;
using System.IO;

namespace Hypermint.Base
{
    public class HypermintLogger : ILoggerFacade
    {
        private ILogger _logger;

        public HypermintLogger(string name = null)
        {
            if (_logger == null)
                CreateLogger(name);
        }

        private void CreateLogger(string name = null)
        {
            var fileName = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                fileName = "HM2Log.log";
            }
            else
            {
                fileName = $"{name}.log";
            }
#if DEBUG
                ILoggerFactory logFactory = new LoggerFactory()
                //.AddConsole(LogLevel.Trace)
                .AddFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Hypermint", fileName), LogLevel.Debug);
#else
            ILoggerFactory logFactory = new LoggerFactory()
                .AddFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Hypermint", fileName), LogLevel.Warning);
#endif
            //Create the logger from incoming name.
            if (!string.IsNullOrWhiteSpace(name))
                _logger = logFactory.CreateLogger(name);
            else
                _logger = logFactory.CreateLogger("Hypermint Logger");
        }

        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    _logger?.LogDebug($"{message}", category, priority);
                    break;
                case Category.Exception:
                    _logger?.LogError($"{message}", category, priority);
                    break;
                case Category.Info:
                    _logger?.LogInformation($"{message}", category, priority);
                    break;
                case Category.Warn:
                    _logger?.LogWarning($"{message}", (int)category, priority);
                    break;
                default:
                    break;
            }
        }
    }
}
