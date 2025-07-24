using Application.Interfaces;
using Domain.Entidad.PlantaAlimentoDespacho;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;

namespace WSArisSap.Controllers.PlantaAlimentoDespacho
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantaAlimentoDespachoInsuMaSapController
        (
            ILibraryInitializer libraryInitializer,
            IInitializerContextSAP initializerContextSAP,
            ILoggingService loggingService
        ) : Controller

    {
        [HttpPost("PlantaAlimentoDespachoInsuMa")]
        public IActionResult PlantaAlimentoDespachoInsuMaSap([FromBody] PlantaAlimentoDespachoInsuMa plantaAlimentoDespachoInsuMa)
        {
            try
            {
                loggingService.LogInfo("PlantaAlimentoDespachoInsuMa : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("PlantaAlimentoDespachoInsuMa : Conectando con SAP");
                loggingService.LogInfo($"PlantaAlimentoDespachoInsuMa : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("PlantaAlimentoDespachoInsuMa : Consumiendo RFC ZFM_MM_ENTRADA_MATERIAL");

                using var someFunction = connection.CreateFunction("ZFM_MM_ENTRADA_MATERIAL");

                var result = someFunction.Invoke<PlantaAlimentoDespachoInsuMaResult>(new PlantaAlimentoDespachoInsuMaParameters
                {
                    CABECERA_SAP = new PlantaAlimentoDespachoInsuMaParametersItem
                    {
                        FECHA_RECEPCION = DateTime.ParseExact(plantaAlimentoDespachoInsuMa.FECHA_RECEPCION, "dd.MM.yyyy", null),
                        FECHA_TRANSFERENCIA = DateTime.ParseExact(plantaAlimentoDespachoInsuMa.FECHA_TRANSFERENCIA, "dd.MM.yyyy", null),
                        CLASE_MOVIMIENTO = plantaAlimentoDespachoInsuMa.CLASE_MOVIMIENTO,
                        RECEPCION_ID = plantaAlimentoDespachoInsuMa.RECEPCION_ID
                    },

                    DETALLE_SAP = plantaAlimentoDespachoInsuMa._PlantaAlimentoDespachoInsuMaDetalle
                        .Select(detalle => new PlantaAlimentoDespachoInsuMaParametersItems
                        {
                            MATERIAL_ID = detalle.MATERIAL_ID,
                            PESO_SALIDA = detalle.PESO_SALIDA,
                            UNIDAD_MEDIDA = detalle.UNIDAD_MEDIDA,
                            CLASE_MOVIMIENTO = detalle.CLASE_MOVIMIENTO,
                            CENTRO_ID = detalle.CENTRO_ID,
                            ALMACEN_ID = detalle.ALMACEN_ID,
                            PROVEEDOR_ID = detalle.PROVEEDOR_ID,
                            LOTE = detalle.LOTE
                        })
                        .ToArray()

                });

                loggingService.LogInfo("PlantaAlimentoDespachoInsuMa : Fin Consumiendo RFC ZFM_MM_ENTRADA_MATERIAL");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"PlantaAlimentoDespachoInsuMa : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
