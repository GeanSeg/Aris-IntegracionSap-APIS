using Application.Interfaces;
using Application.Services;
using Dbosoft.YaNco;
using Domain.Entidad;
using Dominio.Entidad;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;

namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class SapReplicaOdooEntregaController( ILibraryInitializer libraryInitializer,
        IInitializerContextSAP initializerContextSAP, SapResultadoEntrega sapResultado,
        ILoggingService loggingService) : Controller
    {
        [HttpPost(Name = "SapReplicaOdooEntrega")]
        public async Task<IActionResult> ReplicaOdooPedido([FromBody] ReplicaOdooEntrega replicaOdooEntrega)
        {
            try
            {
                loggingService.LogInfo("ReplicaOdooEntrega : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("ReplicaOdooEntrega : Conectando con SAP");
                loggingService.LogInfo($"ReplicaOdooEntrega : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("ReplicaOdooEntrega : Consumiendo RFC ZSD_REPLICA_ODOO_ENTREGA");

                loggingService.LogInfo($"VSTEL: {replicaOdooEntrega.VSTEL}");
                loggingService.LogInfo($"DATBI: {replicaOdooEntrega.DATBI}");
                loggingService.LogInfo($"VBELN: {replicaOdooEntrega.VBELN.PadLeft(10, '0')}");
                loggingService.LogInfo($"WADAT_IST: {replicaOdooEntrega.WADAT_IST}");

                foreach (var item in replicaOdooEntrega.replicaOdooEntregaDetalle)
                {
                    loggingService.LogInfo("--- Detalle ---");
                    loggingService.LogInfo($"POSNR: {item.POSNR.PadLeft(6, '0')}");
                    loggingService.LogInfo($"MATNR: {item.KWMENG}");
                    loggingService.LogInfo($"WERKS: {(item.VRKME == "UN" ? "ST" : item.VRKME)}");
                    loggingService.LogInfo($"WERKS: {item.CHARG}");
                }

                using var someFunction = connection.CreateFunction("ZSD_REPLICA_ODOO_ENTREGA");

                var result = someFunction.Invoke<EntregaResult>(new EntregaParameters
                {
                    CAB = new EntregaResultItemCAB
                    {
                        VSTEL = replicaOdooEntrega.VSTEL,
                        DATBI = DateTime.ParseExact(replicaOdooEntrega.DATBI, "dd.MM.yyyy", null),
                        VBELN = replicaOdooEntrega.VBELN.PadLeft(10, '0'),
                        WADAT_IST = DateTime.ParseExact(replicaOdooEntrega.WADAT_IST, "dd.MM.yyyy", null)
                    },
                    DET = replicaOdooEntrega.replicaOdooEntregaDetalle
                        .Select(detalle => new EntregaResultItemDET
                        {
                            POSNR = detalle.POSNR.PadLeft(6, '0'),
                            KWMENG = detalle.KWMENG,
                            VRKME = detalle.VRKME == "UN"? "ST" : detalle.VRKME,
                            CHARG = detalle.CHARG
                        })
                        .ToArray()
                });

                loggingService.LogInfo("ReplicaOdooEntrega : Fin Consumiendo RFC ZSD_REPLICA_ODOO_ENTREGA");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"ReplicaOdooEntrega : {ex.Message}");
                return StatusCode(500, new {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
