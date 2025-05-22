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
    public sealed class SapReplicaOdooFacturaController( ILibraryInitializer libraryInitializer,
        IInitializerContextSAP initializerContextSAP, SapResultadoFactura sapResultado,
        ILoggingService loggingService) : Controller
    {
        [HttpPost(Name = "SapReplicaOdooFactura")]
        public async Task<IActionResult> ReplicaOdooPedido([FromBody] ReplicaOdooFactura replicaOdooFactura)
        {
            try
            {
                loggingService.LogInfo("ReplicaOdooFactura : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("ReplicaOdooFactura : Conectando con SAP");
                loggingService.LogInfo($"ReplicaOdooFactura : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("ReplicaOdooFactura : Consumiendo RFC ZSD_REPLICA_ODOO_FACTURA");
                loggingService.LogInfo($"I_ENTREGA: {replicaOdooFactura.I_ENTREGA}");

                using var someFunction = connection.CreateFunction("ZSD_REPLICA_ODOO_FACTURA");

                var result = someFunction.Invoke<FacturaResult>(new FacturaParameters
                {
                    I_ENTREGA = replicaOdooFactura.I_ENTREGA
                ,
                });

                loggingService.LogInfo("ReplicaOdooFactura : Fin Consumiendo RFC ZSD_REPLICA_ODOO_FACTURA");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"ReplicaOdooFactura : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
