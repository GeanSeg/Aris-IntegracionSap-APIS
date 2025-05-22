using Application.Interfaces;
using Domain.Entidad;
using Domain.Entidad.GestionPedidosCobranzas;
using Domain.Entidad.GestionPedidosCobranzasAris;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;

namespace WSpruebaArisSap.Controllers.GestionPedidosCobranzasArisController
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CobranzaFacturaArisController(ILibraryInitializer libraryInitializer,
         IInitializerContextSAP initializerContextSAP,
         ILoggingService loggingService) : Controller
    {
        [HttpPost(Name = "CobranzaFacturaAris")]
        public async Task<IActionResult> CobranzaFacturaAris([FromBody] CobranzaFacturaAris cobranzaFacturaAris)
        {
            try
            {
                loggingService.LogInfo("CobranzaFacturaAris : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("CobranzaFacturaAris : Conectando con SAP");
                loggingService.LogInfo($"CobranzaFacturaAris : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("CobranzaFacturaAris : Consumiendo RFC ZFI_F_COBRANZA_CON_FACTURA");

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

                using var someFunction = connection.CreateFunction("ZFI_F_COBRANZA_CON_FACTURA");

                var result = someFunction.Invoke<CobranzaFacturaArisResult>(new CobranzaFacturaArisParameters
                {
                    I_COBRANZA_CAB = new CobranzaFacturaArisParametersItem
                    {
                        BLDAT = DateTime.ParseExact(cobranzaFacturaAris.BLDAT, "dd.MM.yyyy", null),
                        BUDAT = DateTime.ParseExact(cobranzaFacturaAris.BUDAT, "dd.MM.yyyy", null),
                        BLART = cobranzaFacturaAris.BLART,
                        BUKRS = cobranzaFacturaAris.BUKRS,
                        WAERS = cobranzaFacturaAris.WAERS,
                        BKTXT = cobranzaFacturaAris.BKTXT,
                        KONTO = cobranzaFacturaAris.KONTO,
                        VALUT = DateTime.ParseExact(cobranzaFacturaAris.VALUT, "dd.MM.yyyy", null),
                        AGKON = cobranzaFacturaAris.AGKON
                    },

                    T_COBRANZA_DET = cobranzaFacturaAris.cobranzaFacturaArisDetalle
                        .Select(detalle => new CobranzaFacturaArisParametersItems
                        {
                            NRO_FACTURA =detalle.NRO_FACTURA,
                            IMPORTE_PAGO = detalle.IMPORTE_PAGO,
                            REFERENCIA = detalle.REFERENCIA,
                            SGTXT = detalle.SGTXT,
                            ZUONR = detalle.ZUONR,
                            SGTXT_COMP = detalle.SGTXT_COMP
                        })
                        .ToArray()

                });

                loggingService.LogInfo("CobranzaFacturaAris : Fin Consumiendo RFC ZFI_F_COBRANZA_CON_FACTURA");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"CobranzaFacturaAris : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
