using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class LoggerServicio : ILoggingService
    {
        private readonly ILogger<LoggerServicio> _logger;

        public LoggerServicio(ILogger<LoggerServicio> logger)
        {
            _logger = logger;
        }
        public void LogInfo(string mensaje) => _logger.LogInformation(mensaje);

        public void LogError(string mensaje) => _logger.LogError(mensaje);
    }
}
