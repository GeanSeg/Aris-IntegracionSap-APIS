using Application.Interfaces;
using Domain.Entidad.GestionPedidosCobranzasAris;
using Domain.Entidad.PlantaAlimentoDespacho;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;

namespace WSArisSap.Controllers.PlantaAlimentoDespachoSap
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ConsultaOrdenesCompraSapController
        (
            ILibraryInitializer libraryInitializer,
            IInitializerContextSAP initializerContextSAP,
            ILoggingService loggingService
        ): Controller
    {
        [HttpPost("ConsultaOrdenesCompra")]
        public IActionResult ConsultaOrdenesCompraSap([FromBody] ConsultaOrdenesCompra consultaOrdenesCompra)
        {
            try
            {
                loggingService.LogInfo("ConsultaOrdenesCompraSap : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("ConsultaOrdenesCompraSap : Conectando con SAP");
                loggingService.LogInfo($"ConsultaOrdenesCompraSap : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("ConsultaOrdenesCompraSap : Consumiendo RFC ZMM_FM_CONSULTA_PEDIDO");

                using var someFunction = connection.CreateFunction("ZMM_FM_CONSULTA_PEDIDO");

                var result = someFunction.Invoke<ConsultaOrdenesCompraResult>(new ConsultaOrdenesCompraParameters
                {
                    I_FEC_CREA_INICIO = DateTime.ParseExact(consultaOrdenesCompra.I_FEC_CREA_INICIO, "dd.MM.yyyy", null),
                    I_FEC_CREA_FIN = DateTime.ParseExact(consultaOrdenesCompra.I_FEC_CREA_FIN, "dd.MM.yyyy", null),
                    I_NRO_PEDIDO = string.IsNullOrEmpty(consultaOrdenesCompra.I_NRO_PEDIDO) ? "" : consultaOrdenesCompra.I_NRO_PEDIDO,
                    I_SOCIEDAD = string.IsNullOrEmpty(consultaOrdenesCompra.I_SOCIEDAD) ? "" : consultaOrdenesCompra.I_SOCIEDAD

                }); 

                loggingService.LogInfo("ConsultaOrdenesCompraSap : Fin Consumiendo RFC ZMM_FM_CONSULTA_PEDIDO");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"ConsultaOrdenesCompraSap : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
