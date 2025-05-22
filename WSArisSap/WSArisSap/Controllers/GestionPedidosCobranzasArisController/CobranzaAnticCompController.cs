using Application.Interfaces;
using Domain.Entidad.GestionPedidosCobranzasAris;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;

namespace WSpruebaArisSap.Controllers.GestionPedidosCobranzasArisController
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CobranzaAnticCompController(ILibraryInitializer libraryInitializer,
         IInitializerContextSAP initializerContextSAP,
         ILoggingService loggingService) : Controller
    {
        [HttpPost(Name = "CobranzaAnticComp")]
        public async Task<IActionResult> CobranzaAnticComp([FromBody] CobranzaAnticCompAris cobranzaAnticCompAris)
        {
            try
            {
                loggingService.LogInfo("CobranzaAnticComp : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("CobranzaAnticComp : Conectando con SAP");
                loggingService.LogInfo($"CobranzaAnticComp : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("CobranzaAnticComp : Consumiendo RFC ZFI_F_COBRANZA_ANTIC_COMP.");

                //loggingService.LogInfo($"VSTEL: {replicaOdooEntrega.VSTEL}");
                //loggingService.LogInfo($"DATBI: {replicaOdooEntrega.DATBI}");
                //loggingService.LogInfo($"VBELN: {replicaOdooEntrega.VBELN.PadLeft(10, '0')}");
                //loggingService.LogInfo($"WADAT_IST: {replicaOdooEntrega.WADAT_IST}");

                //foreach (var item in replicaOdooEntrega.replicaOdooEntregaDetalle)
                //{
                //    loggingService.LogInfo("--- Detalle ---");
                //    loggingService.LogInfo($"POSNR: {item.POSNR.PadLeft(6, '0')}");
                //    loggingService.LogInfo($"MATNR: {item.KWMENG}");
                //    loggingService.LogInfo($"WERKS: {(item.VRKME == "UN" ? "ST" : item.VRKME)}");
                //    loggingService.LogInfo($"WERKS: {item.CHARG}");
                //}

                using var someFunction = connection.CreateFunction("ZFI_F_COBRANZA_ANTIC_COMP");

                var result = someFunction.Invoke<CobranzaAnticCompArisResult>(new CobranzaAnticCompArisParameters
                {
                    I_COBRANZA_ANT = new CobranzaAnticCompArisParametersItem
                    {
                        BLDAT = DateTime.ParseExact(cobranzaAnticCompAris.BLDAT, "dd.MM.yyyy", null),
                        BUDAT = DateTime.ParseExact(cobranzaAnticCompAris.BUDAT, "dd.MM.yyyy", null),
                        BLART = cobranzaAnticCompAris.BLART,
                        BUKRS = cobranzaAnticCompAris.BUKRS,
                        WAERS = cobranzaAnticCompAris.WAERS,
                        REFERENCIA = cobranzaAnticCompAris.REFERENCIA,
                        NRO_FACTURA = cobranzaAnticCompAris.NRO_FACTURA,
                        BKTXT = cobranzaAnticCompAris.BKTXT,
                        NEWKO = cobranzaAnticCompAris.NEWKO
                    },

                    T_COBRANZA_DET_ANT = cobranzaAnticCompAris.cobranzaAnticCompArisDetalles
                        .Select(detalle => new CobranzaAnticCompArisParametersItems
                        {
                            REF_ANTICIPO = detalle.REF_ANTICIPO
                        })
                        .ToArray()

                });

                loggingService.LogInfo("CobranzaAnticComp : Fin Consumiendo RFC ZFI_F_COBRANZA_ANTIC_COMP");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"CobranzaAnticComp : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
