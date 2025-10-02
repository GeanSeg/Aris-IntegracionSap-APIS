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
    public class NotificacionConsumoProduccionController : ControllerBase
    {
        public class NotConsPRDOiItem
        {
            public string MATNR { get; set; }
            public string WERKS { get; set; }
            public string LGORT { get; set; }
            public string CHARG { get; set; }
            public string BWART { get; set; }
            public string ENTRY_QNT { get; set; }
            public string ENTRY_UOM { get; set; }
        }

        public class NotConsPRDOiRequest
        {
            public string E_TYPE_NOTIF { get; set; }
            public string MATNR { get; set; }
            public string WERKS { get; set; }
            public string PLWERK { get; set; }
            public string LGORT { get; set; }
            public string VERID { get; set; }
            public string CHARG { get; set; }
            public string BUDAT { get; set; }
            public string BLDAT { get; set; }
            public string REFMG { get; set; }
            public string ERFME { get; set; }
            public string UARIS_CREA { get; set; }
            public string UARIS_MOD { get; set; }
            public List<NotConsPRDOiItem> Items { get; set; }
        }

        private readonly IConfiguration _configuration;

        public NotificacionConsumoProduccionController(IConfiguration configuration)
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

        [HttpPost("NotificacionConsumoProduccionController")]
        public async Task<IActionResult> CreateNotificacionConsumoProduccion([FromBody] NotConsPRDOiRequest request)

        {
            
            if (request == null)
            {
                return BadRequest(new { Error = "El cuerpo de la solicitud no puede estar vacío." });
            }

            // Validate mandatory fields
            if (string.IsNullOrEmpty(request.BUDAT) || string.IsNullOrEmpty(request.BLDAT))
            {
                return BadRequest(new { Error = "BUDAT y BLDAT son obligatorios." });
            }

            // Validate date format
            if (!DateTime.TryParseExact(request.BUDAT, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedBudat))
            {
                return BadRequest(new { Error = "Formato de BUDAT inválido. Use dd.MM.yyyy." });
            }
            if (!DateTime.TryParseExact(request.BLDAT, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedBldat))
            {
                return BadRequest(new { Error = "Formato de BLDAT inválido. Use dd.MM.yyyy." });
            }

            // Validate items
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { Error = "La lista de ítems no puede estar vacía." });
            }

            // Load SAP connection settings from configuration
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
                    // Assign default values for nullable fields
                    string eTypeNotif = string.IsNullOrEmpty(request.E_TYPE_NOTIF) ? "" : request.E_TYPE_NOTIF;
                    string matnr = string.IsNullOrEmpty(request.MATNR) ? "" : request.MATNR;
                    string werks = string.IsNullOrEmpty(request.WERKS) ? "" : request.WERKS;
                    string plwerk = string.IsNullOrEmpty(request.PLWERK) ? "" : request.PLWERK;
                    string lgort = string.IsNullOrEmpty(request.LGORT) ? "" : request.LGORT;
                    string verid = string.IsNullOrEmpty(request.VERID) ? "" : request.VERID;
                    string charg = string.IsNullOrEmpty(request.CHARG) ? "" : request.CHARG;
                    string refmg = string.IsNullOrEmpty(request.REFMG) ? "" : request.REFMG;
                    string erfme = string.IsNullOrEmpty(request.ERFME) ? "" : request.ERFME;
                    string uarisCrea = string.IsNullOrEmpty(request.UARIS_CREA) ? "" : request.UARIS_CREA;
                    string uarisMod = string.IsNullOrEmpty(request.UARIS_MOD) ? "" : request.UARIS_MOD;

                    var result = await context.CallFunction("ZPP_FM_NOTIF_PROD_ORDEN_FAB",
                        Input: f => f
                            .SetField("E_TYPE_NOTIF", eTypeNotif)
                            .SetStructure("ES_FLUSHDATAGEN", s => s
                                .SetField("MATNR", FormatMatnr(matnr))
                                .SetField("WERKS", werks)
                                .SetField("PLWERK", plwerk)
                                .SetField("LGORT", lgort)
                                .SetField("VERID", verid)
                                .SetField("CHARG", charg)
                                .SetField("BUDAT", parsedBudat)
                                .SetField("BLDAT", parsedBldat)
                                .SetField("REFMG", refmg)
                                .SetField("ERFME", erfme == "UN" ? "ST" : erfme)
                                .SetField("UARIS_CREA", uarisCrea)
                                .SetField("UARIS_MOD", uarisMod))
                            .SetTable("IT_GOODSMOVEMENT", request.Items, (structure, item) => structure
                                .SetField("MATNR", FormatMatnr(item.MATNR))
                                .SetField("WERKS", item.WERKS)
                                .SetField("LGORT", item.LGORT)
                                .SetField("CHARG", item.CHARG)
                                .SetField("BWART", item.BWART)
                                .SetField("ENTRY_QNT", string.IsNullOrWhiteSpace(item.ENTRY_QNT) ? 0.000m : Convert.ToDecimal(item.ENTRY_QNT, System.Globalization.CultureInfo.InvariantCulture))
                                .SetField("ENTRY_UOM", item.ENTRY_UOM == "UN" ? "ST" : item.ENTRY_UOM)),
                        Output: f => (
                            from E_NUMB_NOTIF in f.GetField<string>("E_NUMB_NOTIF")
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
                                E_NUMB_NOTIF,
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
                    // Log exception (e.g., using ILogger in production)
                    return BadRequest(new { Error = $"Ocurrió un error al procesar la solicitud: {ex.Message}" });
                }
            }
        }
    }
}