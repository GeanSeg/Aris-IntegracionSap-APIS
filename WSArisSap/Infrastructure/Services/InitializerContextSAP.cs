using Application.Interfaces;
using Dbosoft.YaNco;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public sealed class InitializerContextSAP : IInitializerContextSAP 
    {
        private readonly SapSettings _settings;
        private readonly ILoggingService _loggerServicio;

        public InitializerContextSAP(ILoggingService loggingService , IOptions<SapSettings> options)
        {
            _settings = options.Value;
            _loggerServicio = loggingService;

        }

        public string InitializeContextConnSap()
        {
            string connectionString = "";
            try
            {
                _loggerServicio.LogInfo("InitializeContextConnSap : Inicializando credenciales");

                connectionString = $"AppServerHost={_settings.AppServerHost}; " +
                          $"SystemNumber={_settings.SystemNumber}; " +
                          $"User={_settings.User}; " +
                          $"Password={_settings.Password}; " +
                          $"Client={_settings.Client}; " +
                          $"Language={_settings.Language}; " +
                          $"PoolSize={_settings.PoolSize}; " +
                          $"Trace={_settings.Trace}";
            }
            catch (Exception ex) {
                connectionString = "";
                _loggerServicio.LogError($"InitializeContextConnSap :{ex.Message}");
            }

            return connectionString;
        }
    }
}
