using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dbosoft.YaNco.TypeMapping;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using LanguageExt.ClassInstances.Pred;

namespace WSArisSap.Controllers.IntegracionMaestrosController
{
    [ApiController]
    [Route("api/")]
    public class ObtenerAsignacionClientesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ObtenerAsignacionClientesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string FormatKnnr(string KnnrInput)
        {
            const int knnrLength = 10;

            // Si el valor es nulo o vacío, devolver cadena vacía
            if (string.IsNullOrWhiteSpace(KnnrInput))
            {
                return "";
            }

            // Limpiar espacios en blanco
            string trimmedKnnr = KnnrInput.Trim();

            // Verificar si el valor comienza con una letra
            if (char.IsLetter(trimmedKnnr[0]))
            {

                return trimmedKnnr;
            }

            // Verificar si el valor es numérico
            if (trimmedKnnr.All(char.IsDigit))
            {
                // Formatear con ceros a la izquierda hasta 18 caracteres
                string formattedKnnr = trimmedKnnr.PadLeft(knnrLength, '0');

                return formattedKnnr;
            }

            // Si no es numérico ni comienza con letra, devolver tal cual con log de advertencia

            return trimmedKnnr;
        }

        [HttpGet("ObtenerAsignacionClientesController")]
        public async Task<IActionResult> GetObtenerAsignacionClientes(
            string I_KUNNR = "",
            string I_VKORG = "",
            string I_VTWEG = "",
            string I_SPART = ""
            )
        {

            string formattedMatnr = FormatKnnr(I_KUNNR);

            string basePath = Path.Combine(AppContext.BaseDirectory, "Recursos");
            NativeLibrary.Load(Path.Combine(basePath, "icuuc50.dll"));
            NativeLibrary.Load(Path.Combine(basePath, "icudt50.dll"));
            NativeLibrary.Load(Path.Combine(basePath, "icuin50.dll"));
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

            var connectionBuilder = new ConnectionBuilder(settings);
            var connFunc = connectionBuilder.Build();

            using (var context = new RfcContext(connFunc))
            {
                try
                {

                    //I_KUNNR = string.IsNullOrEmpty(I_KUNNR) ? "" : I_KUNNR;
                    I_VKORG = string.IsNullOrEmpty(I_VKORG) ? "" : I_VKORG;
                    I_VTWEG = string.IsNullOrEmpty(I_VTWEG) ? "" : I_VTWEG;
                    I_SPART = string.IsNullOrEmpty(I_SPART) ? "" : I_SPART;

                    var result = await context.CallFunction("ZSD_FM_MIG_ASIG_CLIENT",
                        Input: f => f
                            .SetField("I_KUNNR", formattedMatnr)
                            .SetField("I_VKORG", I_VKORG)
                            .SetField("I_VTWEG", I_VTWEG)
                            .SetField("I_SPART", I_SPART),

                        Output: f => (
                            from T_ASIG_CLIENT in f.MapTable("T_ASIG_CLIENT", s =>
                                from KUNNR in s.GetField<string>("KUNNR")
                                from VKORG in s.GetField<string>("VKORG")
                                from VTWEG in s.GetField<string>("VTWEG")
                                from SPART in s.GetField<string>("SPART")
                                from PARVW in s.GetField<string>("PARVW")
                                from KTONR in s.GetField<string>("KTONR")
                                from DEFPA in s.GetField<string>("DEFPA")

                                select new
                                {

                                    KUNNR,
                                    VKORG,
                                    VTWEG,
                                    SPART,
                                    PARVW,
                                    KTONR,
                                    DEFPA,
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
                                T_ASIG_CLIENT,
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
                    return BadRequest(new { Error = ex.Message });
                }
            }
        }
    }
}