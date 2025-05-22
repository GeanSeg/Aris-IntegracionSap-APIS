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
using LanguageExt.Pipes;
using LanguageExt;



namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("api/")]
    public class NotificacionConsumoProduccionController : ControllerBase
    {
        public class NotConsOiItem
        {
            public string MATNR { get; set; }
            public string WERKS { get; set; }
            public string LGORT { get; set; }
            public string CHARG { get; set; }
            public string BWART { get; set; }
            public string ENTRY_QNT { get; set; }
            public string ENTRY_UOM { get; set; }
            
        }

        private readonly IConfiguration _configuration;

        public NotificacionConsumoProduccionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("NotificacionConsumoProduccionController")]
        public async Task<IActionResult> GetNotificacionConsumoProduccion(string E_TYPE_NOTIF ="",
             string MATNR = "", string WERKS = "",string PLWERK="", string LGORT = "", string VERID = "", 
             string CHARG = "",string BUDAT="",string BLDAT ="",
             string REFMG = "",string ERFME="",
             string UARIS_CREA = "", string UARIS_MOD = "",
             string matnr="",string werks = "", string lgort = "",string charg = "",string bwart = "",string entry_qnt = "",string entry_uom = ""
             )
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
                    E_TYPE_NOTIF = string.IsNullOrEmpty(E_TYPE_NOTIF) ? "" : E_TYPE_NOTIF;
                    MATNR = string.IsNullOrEmpty(MATNR) ? "" : MATNR;
                    WERKS = string.IsNullOrEmpty(WERKS) ? "" : WERKS;
                    PLWERK = string.IsNullOrEmpty(PLWERK) ? "" : PLWERK;
                    LGORT = string.IsNullOrEmpty(LGORT) ? "" : LGORT;
                    VERID = string.IsNullOrEmpty(VERID) ? "" : VERID;
                    CHARG = string.IsNullOrEmpty(CHARG) ? "" : CHARG;
                    BUDAT = string.IsNullOrEmpty(BUDAT) ? "" : BUDAT;
                    BLDAT = string.IsNullOrEmpty(BLDAT) ? "" : BLDAT;
                    REFMG = string.IsNullOrEmpty(REFMG) ? "" : REFMG;
                    ERFME = string.IsNullOrEmpty(ERFME) ? "" : ERFME;
                    UARIS_CREA = string.IsNullOrEmpty(UARIS_CREA) ? "" : UARIS_CREA;
                    UARIS_MOD = string.IsNullOrEmpty(UARIS_MOD) ? "" : UARIS_MOD;
                    matnr = string.IsNullOrEmpty(matnr) ? "" : matnr;
                    werks = string.IsNullOrEmpty(werks) ? "" : werks;
                    lgort = string.IsNullOrEmpty(lgort) ? "" : lgort;
                    charg = string.IsNullOrEmpty(charg) ? "" : charg;
                    bwart = string.IsNullOrEmpty(bwart) ? "" : bwart;
                    entry_qnt = string.IsNullOrEmpty(entry_qnt) ? "" : entry_qnt;
                    entry_uom = string.IsNullOrEmpty(entry_uom) ? "" : entry_uom;

                    var items = new List<NotConsOiItem>
{
                            new NotConsOiItem
                            {
                                MATNR = matnr,
                                WERKS = werks,
                                LGORT = lgort,
                                CHARG = charg,
                                BWART = bwart,
                                ENTRY_QNT = entry_qnt,
                                ENTRY_UOM = entry_uom
                            }

                    };
                    var result = await context.CallFunction("ZPP_FM_NOTIF_PROD_ORDEN_FAB",
                        Input: f => f
                            .SetField("E_TYPE_NOTIF", E_TYPE_NOTIF)

                            .SetStructure("ES_FLUSHDATAGEN", s => s
                                .SetField("MATNR", MATNR)
                                .SetField("WERKS", WERKS)
                                .SetField("PLWERK", PLWERK)
                                .SetField("LGORT", LGORT)
                                .SetField("VERID", VERID)
                                .SetField("CHARG", CHARG)
                                .SetField("BUDAT", DateTime.ParseExact(BUDAT, "dd.MM.yyyy", null))
                                .SetField("BLDAT", DateTime.ParseExact(BLDAT, "dd.MM.yyyy", null))
                                .SetField("REFMG", REFMG)
                                .SetField("ERFME", ERFME)
                                .SetField("UARIS_CREA", UARIS_CREA)
                                .SetField("UARIS_MOD", UARIS_MOD))

                        .SetTable("IT_GOODSMOVEMENT", items,
                                         (structure, items) => structure
                                                 .SetField("MATNR", items.MATNR)
                                                 .SetField("WERKS", items.WERKS)
                                                 .SetField("LGORT", items.LGORT)
                                                 .SetField("CHARG", items.CHARG)
                                                 .SetField("BWART", items.BWART)
                                                 .SetField("ENTRY_QNT", string.IsNullOrWhiteSpace(items.ENTRY_QNT) ? 0.000m : Convert.ToDecimal(items.ENTRY_QNT, System.Globalization.CultureInfo.InvariantCulture))
                                                 .SetField("ENTRY_UOM", items.ENTRY_UOM)),

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
                    return BadRequest(new { Error = ex.Message });
                }
            }
        }
    }
}