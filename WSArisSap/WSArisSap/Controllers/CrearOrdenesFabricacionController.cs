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
    public class CrearOrdenesFabricacionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CrearOrdenesFabricacionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("CrearOrdenesFabricacionController")]
        public async Task<IActionResult> GetUpdateOrdenInversion(string MATNR="", string WERKS = "", string AUFART = "" , decimal? GAMNG = null, string START_DATE = "", string END_DATE = "", string DISPO = "", string FEVOR = "",
            string INSMK = "", string LGORT = "", string CHARG = "")
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
                    MATNR = string.IsNullOrEmpty(MATNR) ? "" : MATNR;
                    WERKS = string.IsNullOrEmpty(WERKS) ? "" : WERKS;
                    AUFART = string.IsNullOrEmpty(AUFART) ? "" : AUFART;
                    
                    START_DATE = string.IsNullOrEmpty(START_DATE) ? "" : START_DATE;
                    END_DATE = string.IsNullOrEmpty(END_DATE) ? "" : END_DATE;
                    DISPO = string.IsNullOrEmpty(DISPO) ? "" : DISPO;
                    FEVOR = string.IsNullOrEmpty(FEVOR) ? "" : FEVOR;
                    INSMK = string.IsNullOrEmpty(INSMK) ? "" : INSMK;
                    LGORT = string.IsNullOrEmpty(LGORT) ? "" : LGORT;
                    CHARG = string.IsNullOrEmpty(CHARG) ? "" : CHARG;

                    var result = await context.CallFunction("ZPP_FM_CREATE_ORDEN_FAB",
                        Input: f => f.SetStructure("IS_DAT_ORDEN_FAB", s => s
                                        .SetField("MATNR", MATNR)
                                        .SetField("WERKS", WERKS)
                                        .SetField("AUFART", AUFART)
                                        .SetField("GAMNG", GAMNG)
                                        .SetField("START_DATE", DateTime.ParseExact(START_DATE, "dd.MM.yyyy", null))
                                        .SetField("END_DATE", DateTime.ParseExact(END_DATE, "dd.MM.yyyy", null))
                                        .SetField("DISPO", DISPO)
                                        .SetField("FEVOR", FEVOR)
                                        .SetField("INSMK", INSMK)
                                        .SetField("LGORT", LGORT)
                                        .SetField("CHARG", CHARG)), 

                        Output: f => (
                        from E_ORDER_NUMBER in f.GetField<string>("E_ORDER_NUMBER")
                   
                        from E_STATUS_ORDER in f.GetField<string>("E_STATUS_ORDER")

                        from T_RETURN in f.MapTable("T_RETURN", s =>
                            from TYPE in s.GetField<decimal>("TYPE")
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
                            E_ORDER_NUMBER,
                            E_STATUS_ORDER,
                            T_RETURN
                            }));

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