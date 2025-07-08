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
                ConsultaOrdenesCompraResult consultaOrdenesCompraResult;
                consultaOrdenesCompraResult = new ConsultaOrdenesCompraResult();

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

                consultaOrdenesCompraResult = result;

                loggingService.LogInfo("ConsultaOrdenesCompraSap : Fin Consumiendo RFC ZMM_FM_CONSULTA_PEDIDO");

                if (string.IsNullOrEmpty(consultaOrdenesCompra.VC_CLASE_DOCUMENTO))
                {
                    var consultaOrdenesCompraFiltro = new ConsultaOrdenesCompraResult
                    {
                        ET_CABECERA = consultaOrdenesCompraResult.ET_CABECERA,
                        ET_DETALLE = consultaOrdenesCompraResult.ET_DETALLE,
                        ET_RETURN = consultaOrdenesCompraResult.ET_RETURN,
                    };
                    return Ok(consultaOrdenesCompraFiltro);
                }
                else
                {
                    var clasesDocumento = string.IsNullOrWhiteSpace(consultaOrdenesCompra.VC_CLASE_DOCUMENTO)
                        ? new List<string>() 
                        : consultaOrdenesCompra.VC_CLASE_DOCUMENTO
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(c => c.Trim().ToUpper())
                            .ToList();

                    var ebelnsCabecera = consultaOrdenesCompraResult.ET_CABECERA
                        .Where(c => !clasesDocumento.Any() || clasesDocumento.Contains(c.BSART.ToUpper()))
                        .Select(c => c.EBELN)
                        .ToHashSet();

                    var consultaOrdenesCompraFiltro = new ConsultaOrdenesCompraResult
                    {
                        ET_CABECERA = consultaOrdenesCompraResult.ET_CABECERA
                            .Where(c => !clasesDocumento.Any() || clasesDocumento.Contains(c.BSART.ToUpper()))
                            .ToArray(),

                        ET_DETALLE = consultaOrdenesCompraResult.ET_DETALLE
                            .Where(d => ebelnsCabecera.Contains(d.EBELN))
                            .ToArray(),

                        ET_RETURN = consultaOrdenesCompraResult.ET_RETURN
                    };

                    return Ok(consultaOrdenesCompraFiltro);
                }

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
