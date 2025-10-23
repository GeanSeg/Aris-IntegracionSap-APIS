using Application.Interfaces;
using Domain.Entidad.UpdateXBLNR;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;

namespace WSpruebaArisSap.Controllers.UpdateXBLNRController
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class UpdateXBLNRC(ILibraryInitializer libraryInitializer,
         IInitializerContextSAP initializerContextSAP,
         ILoggingService loggingService) : Controller
    {
        [HttpPost(Name = "UpdateXBLNR")]
        public async Task<IActionResult> mUpdateXBLNR([FromBody] UpdateXBLNR updateXBLNR)
        {
            try
            {
                loggingService.LogInfo("UpdateXBLNR : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("UpdateXBLNR : Conectando con SAP");
                loggingService.LogInfo($"UpdateXBLNR : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("UpdateXBLNR : Consumiendo RFC ZFM_SD_UPDATE_XBLNR_IN_LIKP");

                using var someFunction = connection.CreateFunction("ZFM_SD_UPDATE_XBLNR_IN_LIKP");

                 var result =  someFunction.Invoke<UpdateXBLNR_Result>(new UpdateXBLNR_Parameters
                {
                   I_VBELN= updateXBLNR.I_VBELN.PadLeft(10,'0'),
                   I_XBLNR=updateXBLNR.I_XBLNR
                 });

                loggingService.LogInfo("UpdateXBLNR : Fin Consumiendo RFC ZFM_SD_UPDATE_XBLNR_IN_LIKP");
                return Ok(result);
            }

            catch (Exception ex)
            {
                loggingService.LogError($"UpdateXBLNR : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
