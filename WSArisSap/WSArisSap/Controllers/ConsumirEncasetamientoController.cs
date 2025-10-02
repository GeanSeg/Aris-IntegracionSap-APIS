using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;
using System.Linq;

namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ConsumirEncasetamientoController : ControllerBase
    {
        public class ConsEncasetItem
        {
            public string MATNR { get; set; }
            public string WERKS { get; set; }
            public string LGORT { get; set; }
            public string CHARG { get; set; }
            public string REFMG { get; set; }
            public string AUFNR { get; set; }
        }

        public class ConsEncasetRequest
        {
            public string BUDAT { get; set; }
            public string BLDAT { get; set; }
            public string UARIS_CREA { get; set; }
            public string UARIS_MOD { get; set; }
            public string I_MOV_TYPE { get; set; }
            public List<ConsEncasetItem> Items { get; set; }
        }

        private readonly IConfiguration _configuration;

        public ConsumirEncasetamientoController(IConfiguration configuration)
        {
            _configuration = configuration;
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
               
                return trimmedMatnr;
            }

            // Verificar si el valor es numérico
            if (trimmedMatnr.All(char.IsDigit))
            {
                // Formatear con ceros a la izquierda hasta 18 caracteres
                string formattedMatnr = trimmedMatnr.PadLeft(matnrLength, '0');
             
                return formattedMatnr;
            }

            // Si no es numérico ni comienza con letra, devolver tal cual con log de advertencia
           
            return trimmedMatnr;
        }

        [HttpPost("ConsumirEncasetamiento")]
        public async Task<IActionResult> CreateConsumirEncasetamiento([FromBody] ConsEncasetRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { Error = "El cuerpo de la solicitud no puede estar vacío." });
            }

            // Validar campos obligatorios
            if (string.IsNullOrEmpty(request.BUDAT) || string.IsNullOrEmpty(request.BLDAT))
            {
                return BadRequest(new { Error = "BUDAT y BLDAT son obligatorios." });
            }

            // Validar formato de fecha
            if (!DateTime.TryParseExact(request.BUDAT, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedBudat))
            {
                return BadRequest(new { Error = "Formato de BUDAT inválido. Use dd.MM.yyyy." });
            }
            if (!DateTime.TryParseExact(request.BLDAT, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedBldat))
            {
                return BadRequest(new { Error = "Formato de BLDAT inválido. Use dd.MM.yyyy." });
            }



            // Cargar configuración de SAP
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
                    // Asignar valores predeterminados para campos opcionales
                    string budat = request.BUDAT;
                    string bldat = request.BLDAT;
                    string uarisCrea = string.IsNullOrEmpty(request.UARIS_CREA) ? "" : request.UARIS_CREA;
                    string uarisMod = string.IsNullOrEmpty(request.UARIS_MOD) ? "" : request.UARIS_MOD;
                    string iMovType = string.IsNullOrEmpty(request.I_MOV_TYPE) ? "" : request.I_MOV_TYPE;

                    var result = await context.CallFunction("ZPP_FM_NOTIF_CONS_ORDEN_INV",
                        Input: f => f.SetStructure("ES_CAB_NOT_CONS_OI", s => s
                                        .SetField("BUDAT", parsedBudat)
                                        .SetField("BLDAT", parsedBldat)
                                        .SetField("UARIS_CREA", uarisCrea)
                                        .SetField("UARIS_MOD", uarisMod))
                                     .SetTable("IT_NOT_CONS_OI", request.Items,
                                        (structure, item) => structure
                                            .SetField("MATNR", FormatMatnr(item.MATNR))
                                            .SetField("WERKS", item.WERKS)
                                            .SetField("LGORT", item.LGORT)
                                            .SetField("CHARG", item.CHARG)
                                            .SetField("REFMG", string.IsNullOrWhiteSpace(item.REFMG) ? 0.000m : Convert.ToDecimal(item.REFMG, System.Globalization.CultureInfo.InvariantCulture))
                                            .SetField("AUFNR", item.AUFNR))
                                     .SetField("I_MOV_TYPE", iMovType),
                        Output: f => (
                            from E_DOC_MATNR in f.GetField<string>("E_DOC_MATNR")
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
                                E_DOC_MATNR,
                                T_RETURN
                            })
                    );

                    return Ok(new
                    {
                        Data = result.Case
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Error = $"Ocurrió un error al procesar la solicitud: {ex.Message}" });
                }
            }
        }
    }
}