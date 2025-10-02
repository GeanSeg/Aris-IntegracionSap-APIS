using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.IO;

namespace WSArisSap.Controllers.IntegracionMaestrosController
{
    [ApiController]
    [Route("api/")]
    public class ObtenerLoteMaterialController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ObtenerLoteMaterialController> _logger;

        public ObtenerLoteMaterialController(IConfiguration configuration, ILogger<ObtenerLoteMaterialController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string FormatMatnr(string matnrInput)
        {
            const int matnrLength = 18;

            // Si el valor es nulo o vacío, devolver cadena vacía
            if (string.IsNullOrWhiteSpace(matnrInput))
            {
                return "";
            }

            // Limpiar espacios en blanco
            string trimmedMatnr = matnrInput.Trim();

            // Verificar si el valor comienza con una letra
            if (char.IsLetter(trimmedMatnr[0]))
            {
                _logger.LogInformation("MATNR comienza con letra, no se formatea: {MatnrInput}", trimmedMatnr);
                return trimmedMatnr;
            }

            // Verificar si el valor es numérico
            if (trimmedMatnr.All(char.IsDigit))
            {
                // Formatear con ceros a la izquierda hasta 18 caracteres
                string formattedMatnr = trimmedMatnr.PadLeft(matnrLength, '0');
                _logger.LogInformation("MATNR numérico formateado: {FormattedMatnr}", formattedMatnr);
                return formattedMatnr;
            }

            // Si no es numérico ni comienza con letra, devolver tal cual con log de advertencia
            _logger.LogWarning("MATNR no es numérico ni comienza con letra, se envía sin formato: {MatnrInput}", trimmedMatnr);
            return trimmedMatnr;
        }

        [HttpGet("ObtenerLoteMaterialController")]
        public async Task<IActionResult> GetObtenerLoteMaterial(
            string I_MARTNR = "",
            string I_MTART = "",
            string I_BUKRS = "",
            string I_WERKS = "",
            string I_LGORT = "")
        {
            // Validar parámetros de entrada
            //if (string.IsNullOrWhiteSpace(I_WERKS))
            //{
            //    return BadRequest(new { Error = "El parámetro I_WERKS es obligatorio." });
            //}

            // Formatear MATNR usando el nuevo método
            string formattedMatnr = FormatMatnr(I_MARTNR);
            // Cargar DLLs de SAP

            string basePath = Path.Combine(AppContext.BaseDirectory, "Recursos");
            NativeLibrary.Load(Path.Combine(basePath, "icuuc50.dll"));
            NativeLibrary.Load(Path.Combine(basePath, "icudt50.dll"));
            NativeLibrary.Load(Path.Combine(basePath, "icuin50.dll"));

            // Cargar configuración de SAP
            var sapSettings = _configuration.GetSection("SapSettings");

            var settings = new Dictionary<string, string>
            {
                {"ashost", sapSettings["AppServerHost"]},
                {"sysnr", sapSettings["SystemNumber"]},
                {"client", sapSettings["Client"]},
                {"user", sapSettings["User"]},
                {"passwd", sapSettings["Password"]},
                {"lang", sapSettings["Language"]}
            };

            if (settings.Any(s => string.IsNullOrEmpty(s.Value)))
            {
                _logger.LogError("Faltan configuraciones de SAP en appsettings.json");
                return StatusCode(500, new { Error = "Configuración de SAP incompleta." });
            }

            var connectionBuilder = new ConnectionBuilder(settings);
            var connFunc = connectionBuilder.Build();

            using (var context = new RfcContext(connFunc))
            {
                try
                {
                    var result = await context.CallFunction("ZMM_FM_MIG_LOTE",
                        Input: f => f
                            .SetField("I_MARTNR", formattedMatnr)
                            //string.IsNullOrWhiteSpace(I_MARTNR) ? "" : I_MARTNR.PadLeft(18, '0'))
                            .SetField("I_MTART", string.IsNullOrWhiteSpace(I_MTART) ? "" : I_MTART)
                            .SetField("I_BUKRS", string.IsNullOrWhiteSpace(I_BUKRS) ? "" : I_BUKRS)
                            .SetField("I_LGORT", string.IsNullOrWhiteSpace(I_LGORT) ? "" : I_LGORT)
                            .SetTable("I_WERKS", new string[] { I_WERKS },
                                (structure, werksValue) => structure
                                    .SetField("WERKS", string.IsNullOrWhiteSpace(werksValue) ? "" : werksValue)),
                        Output: f => (
                            from T_DATA_CHARG in f.MapTable("T_DATA_CHARG", s =>
                                from MATNR in s.GetField<string>("MATNR")
                                from CHARG in s.GetField<string>("CHARG")
                                from CLABS in s.GetField<string>("CLABS")
                                select new
                                {
                                    MATNR,
                                    CHARG,
                                    CLABS
                                })
                            from T_RETURN in f.MapTable("T_RETURN", s =>
                                  from TYPE in s.GetField<string>("TYPE")
                                  from ID in s.GetField<string>("ID")
                                  from NUMBER in s.GetField<string>("NUMBER")
                                  from MESSAGE in s.GetField<string>("MESSAGE")
                                  from LOG_NO in s.GetField<string>("LOG_NO")
                                  from LOG_MSG_NO in s.GetField<decimal>("LOG_MSG_NO")
                                  from MESSAGE_V1 in s.GetField<string>("MESSAGE_V1")
                                  from MESSAGE_V2 in s.GetField<string>("MESSAGE_V2")
                                  from MESSAGE_V3 in s.GetField<string>("MESSAGE_V3")
                                  from MESSAGE_V4 in s.GetField<string>("MESSAGE_V4")
                                  from PARAMETER in s.GetField<string>("PARAMETER")
                                  from ROW in s.GetField<string>("ROW")
                                  from FIELD in s.GetField<string>("FIELD")
                                  from SYSTEM in s.GetField<string>("SYSTEM")
                                  select new
                                  {
                                      TYPE,
                                      ID,
                                      NUMBER,
                                      MESSAGE,
                                      LOG_NO,
                                      LOG_MSG_NO,
                                      MESSAGE_V1,
                                      MESSAGE_V2,
                                      MESSAGE_V3,
                                      MESSAGE_V4,
                                      PARAMETER,
                                      ROW,
                                      FIELD,
                                      SYSTEM
                                  })
                            select new 
                            { 
                                T_DATA_CHARG,
                                T_RETURN
                            }
                        )
                    );

                    return Ok(new
                    {
                        Data = result.Case
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar la solicitud a SAP: {Message}", ex.Message);
                    return StatusCode(500, new { Error = "Ocurrió un error al procesar la solicitud en SAP." });
                }
            }
        }
    }
}