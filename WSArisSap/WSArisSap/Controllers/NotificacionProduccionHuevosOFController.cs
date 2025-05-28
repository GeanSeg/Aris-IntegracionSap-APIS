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
    public class NotificacionProduccionHuevosOFController : ControllerBase
    {
        public class NotConsConsHuevosPRDItem
        {
            public string MATNR { get; set; }
            public string WERKS { get; set; }
            public string LGORT { get; set; }
            public string CHARG { get; set; }
            public string ENTRY_QNT { get; set; }
        }

        public class NotConsHuevosPRDRequest
        {
            public string BUDAT { get; set; }
            public string AUFNR { get; set; }
            public decimal? YIELD { get; set; }
            public string UARIS_CREA { get; set; }
            public string UARIS_MOD { get; set; }
            public List<NotConsConsHuevosPRDItem> Items { get; set; }
        }

        private readonly IConfiguration _configuration;

        public NotificacionProduccionHuevosOFController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("NotificacionProduccionHuevosOF")]
        public async Task<IActionResult> CreateNotificacionProduccionHuevosOF([FromBody] NotConsHuevosPRDRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { Error = "El cuerpo de la solicitud no puede estar vacío." });
            }

            // Validar campos obligatorios
            if (string.IsNullOrEmpty(request.BUDAT))
            {
                return BadRequest(new { Error = "BUDAT es obligatorio." });
            }

            // Validar formato de fecha
            if (!DateTime.TryParseExact(request.BUDAT, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedBudat))
            {
                return BadRequest(new { Error = "Formato de BUDAT inválido. Use dd.MM.yyyy." });
            }

            // Validar ítems
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { Error = "La lista de ítems no puede estar vacía." });
            }
            foreach (var item in request.Items)
            {
                if (string.IsNullOrEmpty(item.MATNR) || string.IsNullOrEmpty(item.ENTRY_QNT))
                {
                    return BadRequest(new { Error = "Los ítems deben contener MATNR y ENTRY_QNT válidos." });
                }
                if (!decimal.TryParse(item.ENTRY_QNT, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _))
                {
                    return BadRequest(new { Error = $"ENTRY_QNT '{item.ENTRY_QNT}' no es un número válido." });
                }
            }

            // Cargar configuración de SAP
            string basePath = Path.Combine(AppContext.BaseDirectory, "Recursos");
            NativeLibrary.Load(Path.Combine(basePath, "icuuc50.dll"));
            NativeLibrary.Load(Path.Combine(basePath, "icudt50.dll"));
            NativeLibrary.Load(Path.Combine(basePath, "icuin50.dll"));

            var settings = new Dictionary<string, string>
            {
                {"ashost", _configuration["SAP:ASHOST"] ?? "10.45.4.163"},
                {"sysnr", _configuration["SAP:SYSNR"] ?? "01"},
                {"client", _configuration["SAP:CLIENT"] ?? "200"},
                {"user", _configuration["SAP:USER"] ?? "USU_INTEGRAC"},
                {"passwd", _configuration["SAP:PASSWD"] ?? "Rocio*25"},
                {"lang", _configuration["SAP:LANG"] ?? "ES"}
            };

            var connectionBuilder = new ConnectionBuilder(settings);
            var connFunc = connectionBuilder.Build();

            using (var context = new RfcContext(connFunc))
            {
                try
                {
                    // Asignar valores predeterminados para campos opcionales
                    string budat = request.BUDAT;
                    string aufnr = string.IsNullOrEmpty(request.AUFNR) ? "" : request.AUFNR;
                    decimal yieldValue = request.YIELD.HasValue ? request.YIELD.Value : 0.000m;
                    string uarisCrea = string.IsNullOrEmpty(request.UARIS_CREA) ? "" : request.UARIS_CREA;
                    string uarisMod = string.IsNullOrEmpty(request.UARIS_MOD) ? "" : request.UARIS_MOD;

                    var result = await context.CallFunction("ZPP_FM_NOT_PRD_ORDEN_FAB",
                        Input: f => f.SetStructure("ES_CAB_NOT_PRD_OF", s => s
                                        .SetField("BUDAT", parsedBudat)
                                        .SetField("AUFNR", string.IsNullOrWhiteSpace(aufnr) ? "" : "000" + aufnr)
                                        .SetField("YIELD", yieldValue)
                                        .SetField("UARIS_CREA", uarisCrea)
                                        .SetField("UARIS_MOD", uarisMod))
                                     .SetTable("IT_POS_NOT_PRD_OF", request.Items,
                                        (structure, item) => structure
                                            .SetField("MATNR", item.MATNR)
                                            .SetField("WERKS", item.WERKS)
                                            .SetField("LGORT", item.LGORT)
                                            .SetField("CHARG", item.CHARG)
                                            .SetField("ENTRY_QNT", string.IsNullOrWhiteSpace(item.ENTRY_QNT) ? 0.000m : Convert.ToDecimal(item.ENTRY_QNT, System.Globalization.CultureInfo.InvariantCulture))),
                        Output: f => (
                            from E_DOC_MATNR in f.GetField<string>("E_DOC_MATNR")
                            from E_ORDEN_FAB in f.GetField<string>("E_ORDEN_FAB")
                            from E_NOTIF in f.GetField<string>("E_NOTIF")
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
                                E_ORDEN_FAB,
                                E_NOTIF,
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