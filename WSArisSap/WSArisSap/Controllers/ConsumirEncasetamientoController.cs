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
using System.Globalization;
using System.Runtime.InteropServices;

namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ConsumirEncasetamientoController : ControllerBase
    {
        public class NotConsOiItem
        {
            public string MATNR { get; set; }
            public string WERKS { get; set; }
            public string LGORT { get; set; }
            public string CHARG { get; set; }
            public string REFMG { get; set; }
            public string AUFNR { get; set; }
        }

        private readonly IConfiguration _configuration;

        public ConsumirEncasetamientoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("ConsumirEncasetamientoController")]
        public async Task<IActionResult> GetUpdateOrdenInversion(string BUDAT ="", string BLDAT = "", string UARIS_CREA = "", string UARIS_MOD = "", 
            string MATNR = "", string WERKS = "", string LGORT = "", string CHARG = "", string REFMG = "", string AUFNR = "", string I_MOV_TYPE = "")
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
                    BLDAT = string.IsNullOrEmpty(BLDAT) ? "" : BLDAT;
                    UARIS_CREA = string.IsNullOrEmpty(UARIS_CREA) ? "" : UARIS_CREA;
                    UARIS_MOD = string.IsNullOrEmpty(UARIS_MOD) ? "" : UARIS_MOD;
                    MATNR = string.IsNullOrEmpty(MATNR) ? "" : MATNR;
                    WERKS = string.IsNullOrEmpty(WERKS) ? "" : WERKS;
                    LGORT = string.IsNullOrEmpty(LGORT) ? "" : LGORT;
                    CHARG = string.IsNullOrEmpty(CHARG) ? "" : CHARG;
                    REFMG = string.IsNullOrEmpty(REFMG) ? "" : REFMG;
                    AUFNR = string.IsNullOrEmpty(AUFNR) ? "" : AUFNR;
                    I_MOV_TYPE = string.IsNullOrEmpty(I_MOV_TYPE) ? "" : I_MOV_TYPE;

                    var items = new List<NotConsOiItem>
{
                            new NotConsOiItem
                            {
                                MATNR = MATNR,
                                WERKS = WERKS,
                                LGORT = LGORT,
                                CHARG = CHARG,
                                REFMG = REFMG,
                                AUFNR=AUFNR
                            }
                            // Si quieres agregar más filas, simplemente añade más objetos aquí
                    };

                    var result = await context.CallFunction("ZPP_FM_NOTIF_CONS_ORDEN_INV",
                        Input: f => f.SetStructure("ES_CAB_NOT_CONS_OI", s =>s
                        .SetField("BUDAT", DateTime.ParseExact(BUDAT, "dd.MM.yyyy", null))
                        .SetField("BLDAT", DateTime.ParseExact(BLDAT, "dd.MM.yyyy", null))
                        .SetField("UARIS_CREA", UARIS_CREA)
                        .SetField("UARIS_MOD", UARIS_MOD))
                         
                        .SetTable("IT_NOT_CONS_OI", items,
                                         (structure, items) => structure
                                                 .SetField("MATNR", items.MATNR)
                                                 .SetField("WERKS", items.WERKS)
                                                 .SetField("LGORT", items.LGORT)
                                                 .SetField("CHARG", items.CHARG)
                                                 .SetField("REFMG", items.REFMG)
                                                 .SetField("AUFNR", items.AUFNR)
                        )
                       .SetField("I_MOV_TYPE", I_MOV_TYPE),




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
                    return BadRequest(new { Error = ex.Message });
                }
            }
        }
    }
}