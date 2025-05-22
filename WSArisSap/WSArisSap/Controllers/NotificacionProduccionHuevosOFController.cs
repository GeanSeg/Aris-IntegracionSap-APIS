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



namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("api/")]
    public class NotificacionProduccionHuevosOFController : ControllerBase
    {
       public class NotConsOiItem
        {
            public string MATNR { get; set; }
            public string WERKS { get; set; }
            public string LGORT { get; set; }
            public string CHARG { get; set; }
            public string ENTRY_QNT { get; set; }
        }

        private readonly IConfiguration _configuration;

        public NotificacionProduccionHuevosOFController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("NotificacionProduccionHuevosOFController")]
        public async Task<IActionResult> GetNotificacionProduccionHuevosOF(string BUDAT="", string AUFNR = "", decimal ?YIELD=null, string UARIS_CREA = "", string UARIS_MOD = "",
             string MATNR = "", string WERKS = "", string LGORT = "", string CHARG = "", string ENTRY_QNT ="")
        {
            string basePath = Path.Combine(AppContext.BaseDirectory, "Recursos");
            NativeLibrary.Load(Path.Combine(basePath, "icuuc50.dll"));
            NativeLibrary.Load(Path.Combine(basePath, "icudt50.dll"));
            NativeLibrary.Load(Path.Combine(basePath, "icuin50.dll"));
            var settings = new Dictionary<string, string>
            {
                {"ashost", "10.45.4.163"},
                {"sysnr", "01"},
                {"client", "200"},
                {"user", "USU_INTEGRAC"},
                {"passwd","Rocio*25"},
                {"lang", "ES"}
            };

            var connectionBuilder = new ConnectionBuilder(settings);
            var connFunc = connectionBuilder.Build();

            using (var context = new RfcContext(connFunc))
            {
                try
                {
                    BUDAT = string.IsNullOrEmpty(BUDAT) ? "" : BUDAT;
                    AUFNR = string.IsNullOrEmpty(AUFNR) ? "" : AUFNR;
                    AUFNR = string.IsNullOrEmpty(AUFNR) ? "" : AUFNR;
                    UARIS_CREA = string.IsNullOrEmpty(UARIS_CREA) ? "" : UARIS_CREA;
                    UARIS_MOD = string.IsNullOrEmpty(UARIS_MOD) ? "" : UARIS_MOD;
                    MATNR = string.IsNullOrEmpty(MATNR) ? "" : MATNR;
                    WERKS = string.IsNullOrEmpty(WERKS) ? "" : WERKS;
                    LGORT = string.IsNullOrEmpty(LGORT) ? "" : LGORT;
                    CHARG = string.IsNullOrEmpty(CHARG) ? "" : CHARG;
                    ENTRY_QNT = string.IsNullOrEmpty(ENTRY_QNT) ? "" : ENTRY_QNT;

                    var items = new List<NotConsOiItem>
{
                            new NotConsOiItem
                            {
                                MATNR = MATNR,
                                WERKS = WERKS,
                                LGORT = LGORT,
                                CHARG = CHARG,
                                ENTRY_QNT = ENTRY_QNT
                            }

                    };
                    var result = await context.CallFunction("ZPP_FM_NOT_PRD_ORDEN_FAB",
                        Input: f => f.SetStructure("ES_CAB_NOT_PRD_OF", s => s
                                        .SetField("BUDAT", DateTime.ParseExact(BUDAT, "dd.MM.yyyy", null))
                                        .SetField("AUFNR", string.IsNullOrWhiteSpace(AUFNR) ? "" : "000" + AUFNR)//22154  no le ingrso nada
                                        .SetField("YIELD", YIELD.HasValue ? YIELD.Value : 0.000m)
                                        .SetField("UARIS_CREA", UARIS_CREA)
                                        .SetField("UARIS_MOD", UARIS_MOD))
                        .SetTable("IT_POS_NOT_PRD_OF", items,
                                         (structure, items) => structure
                                                 .SetField("MATNR", items.MATNR)
                                                 .SetField("WERKS", items.WERKS)
                                                 .SetField("LGORT", items.LGORT)
                                                 .SetField("CHARG", items.CHARG)
                                                 .SetField("ENTRY_QNT", string.IsNullOrWhiteSpace(items.ENTRY_QNT)  ? 0.000m : Convert.ToDecimal(items.ENTRY_QNT, System.Globalization.CultureInfo.InvariantCulture))),

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
                    return BadRequest(new { Error = ex.Message });
                }
            }
        }
    }
}