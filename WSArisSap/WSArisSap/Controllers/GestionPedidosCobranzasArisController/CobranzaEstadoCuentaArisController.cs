using Application.Interfaces;
using Domain.Entidad;
using Domain.Entidad.GestionPedidosCobranzas;
using Dominio.Entidad;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;

namespace WSpruebaArisSap.Controllers.GestionPedidosCobranzasArisController
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CobranzaEstadoCuentaArisController(ILibraryInitializer libraryInitializer,
         IInitializerContextSAP initializerContextSAP,
         ILoggingService loggingService) : Controller
    {
        [HttpPost(Name = "CobranzaEstadoCuentaAris")]
        public async Task<IActionResult> CobranzasEstadoCuentaAris([FromBody] EstadoCuentaAris estadoCuentaAris)
        {
            try
            {
                loggingService.LogInfo("CobranzaEstadoCuentaAris : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("CobranzaEstadoCuentaAris : Conectando con SAP");
                loggingService.LogInfo($"CobranzaEstadoCuentaAris : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("CobranzaEstadoCuentaAris : Consumiendo RFC ZFI_ESTADO_CUENTA_ARIS");

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

                using var someFunction = connection.CreateFunction("ZFI_ESTADO_CUENTA_ARIS");

                var result = someFunction.Invoke<EstadoCuentaArisResult>(new EstadoCuentaArisParameters
                {
                    FACT_PEND_PAGO = new EstadoCuentaArisParametersItem
                    {
                        DEUDOR =  estadoCuentaAris.DEUDOR.PadLeft(10, '0'),
                        SOCIEDAD =  estadoCuentaAris.SOCIEDAD.PadLeft(4, '0'),
                        FECHA_ABI_FEC_CLAVE = DateTime.ParseExact(estadoCuentaAris.FECHA_ABI_FEC_CLAVE, "dd.MM.yyyy", null),
                    },
                    NRO_FACTURA = string.IsNullOrWhiteSpace(estadoCuentaAris.NRO_FACTURA)? "" : estadoCuentaAris.NRO_FACTURA.PadLeft(10,'0')
                });

                loggingService.LogInfo("CobranzaEstadoCuentaAris : Fin Consumiendo RFC ZFI_ESTADO_CUENTA_ARIS");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"CobranzaEstadoCuentaAris : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
