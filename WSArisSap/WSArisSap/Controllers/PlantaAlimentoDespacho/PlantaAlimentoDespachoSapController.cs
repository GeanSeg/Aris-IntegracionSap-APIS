using Application.Interfaces;
using Domain.Entidad.GestionPedidosCobranzasAris;
using Domain.Entidad.PlantaAlimentoDespacho;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;

namespace WSArisSap.Controllers.PlantaAlimentoDespachoSap
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class PlantaAlimentoDespachoSapController
        (
            ILibraryInitializer libraryInitializer,
            IInitializerContextSAP initializerContextSAP,
            ILoggingService loggingService
        ): Controller
    {
        [HttpPost("PlantaAlimentoDespacho")]
        public IActionResult PlantaAlimentoDespachoSap([FromBody] PlantaAlimentoDespachoEntity plantaAlimentoDespacho)
        {
            try
            {
                loggingService.LogInfo("PlantaAlimentoDespacho : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("PlantaAlimentoDespacho : Conectando con SAP");
                loggingService.LogInfo($"PlantaAlimentoDespacho : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("PlantaAlimentoDespacho : Consumiendo RFC ZMM_FM_MIGO_TRASPASO_CENTROS");

                using var someFunction = connection.CreateFunction("ZMM_FM_MIGO_TRASPASO_CENTROS");

                var result = someFunction.Invoke<PlantaAlimentoDespachoResult>(new PlantaAlimentoDespachoParameters
                {
                    I_PLANTA_ALI_DES_CAB = new PlantaAlimentoDespachoParametersItem
                    {
                        BUDAT = DateTime.ParseExact(plantaAlimentoDespacho.BUDAT, "dd.MM.yyyy", null),
                        BLDAT = DateTime.ParseExact(plantaAlimentoDespacho.BLDAT, "dd.MM.yyyy", null),
                        BKTXT = plantaAlimentoDespacho.BKTXT,
                        XBLNR = plantaAlimentoDespacho.XBLNR
                    },

                    T_PLANTA_ALI_DES_DET = plantaAlimentoDespacho._PlantaAlimentoDespachoDetalle
                        .Select(detalle => new PlantaAlimentoDespachoParametersItems
                        {
                            MATNR = detalle.MATNR,
                            WERKS = detalle.WERKS,
                            LGORT = detalle.LGORT,
                            CHARG = detalle.CHARG,
                            UMMAT = detalle.UMMAT,
                            UMWRK = detalle.UMWRK,
                            UMLGO = detalle.UMLGO,
                            UMCHA = detalle.UMCHA,
                            ENTRY_QNT = detalle.ENTRY_QNT
                        })
                        .ToArray()

                });

                loggingService.LogInfo("PlantaAlimentoDespacho : Fin Consumiendo RFC ZMM_FM_MIGO_TRASPASO_CENTROS");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"PlantaAlimentoDespacho : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
