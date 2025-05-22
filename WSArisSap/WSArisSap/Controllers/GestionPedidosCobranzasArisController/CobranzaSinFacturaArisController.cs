using Application.Interfaces;
using Domain.Entidad.GestionPedidosCobranzasAris;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;

namespace WSpruebaArisSap.Controllers.GestionPedidosCobranzasArisController
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CobranzaSinFacturaArisController(ILibraryInitializer libraryInitializer,
        IInitializerContextSAP initializerContextSAP,
        ILoggingService loggingService) : Controller
    {
        [HttpPost(Name = "CobranzaSinFacturaAris")]
        public async Task<IActionResult> CobranzaSinFacturaAris([FromBody] CobranzaSinFacturaAris cobranzaSinFacturaAris)
        {
            try
            {
                loggingService.LogInfo("CobranzaSinFacturaAris : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("CobranzaSinFacturaAris : Conectando con SAP");
                loggingService.LogInfo($"CobranzaSinFacturaAris : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("CobranzaSinFacturaAris : Consumiendo RFC ZFI_F_COBRANZA_SIN_FACTURA");

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

                using var someFunction = connection.CreateFunction("ZFI_F_COBRANZA_SIN_FACTURA");

                var result = someFunction.Invoke<CobranzaSinFacturaArisResult>(new CobranzaSinFacturaArisParameters
                {
                    I_COBRANZA_CAB = new CobranzaSinFacturaArisParametersItem
                    {
                        BLDAT = DateTime.ParseExact(cobranzaSinFacturaAris.BLDAT, "dd.MM.yyyy", null),
                        BUDAT = DateTime.ParseExact(cobranzaSinFacturaAris.BUDAT, "dd.MM.yyyy", null),
                        BLART = cobranzaSinFacturaAris.BLART,
                        BUKRS = cobranzaSinFacturaAris.BUKRS,
                        WAERS = cobranzaSinFacturaAris.WAERS,
                        XBLNR = cobranzaSinFacturaAris.XBLNR,
                        BKTXT = cobranzaSinFacturaAris.BKTXT,
                        NEWKO = cobranzaSinFacturaAris.NEWKO,
                        UMSKZ = cobranzaSinFacturaAris.UMSKZ,
                        KONTO = cobranzaSinFacturaAris.KONTO,
                        WRBTR = cobranzaSinFacturaAris.WRBTR,
                        VALUT = DateTime.ParseExact(cobranzaSinFacturaAris.VALUT, "dd.MM.yyyy", null),
                        SGTXT = cobranzaSinFacturaAris.SGTXT,
                        ZUONR = cobranzaSinFacturaAris.ZUONR,
                        MWSKZ = cobranzaSinFacturaAris.MWSKZ
                    }
                });

                loggingService.LogInfo("CobranzaSinFacturaAris : Fin Consumiendo RFC ZFI_F_COBRANZA_SIN_FACTURA");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"CobranzaSinFacturaAris : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
