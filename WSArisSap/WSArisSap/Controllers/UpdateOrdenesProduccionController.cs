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
    public class UpdateOrdenesProduccionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UpdateOrdenesProduccionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("UpdateOrdenesProduccionController")]

        public async Task<IActionResult> GetCreateOrdenInversion(string I_ORDERID ="", string I_LIB = "", string I_CTEC = "", string I_REAP = "", string I_BLOQ = "", string I_DESBLOQ = "", string I_CERR = "", string I_BORR = "", string I_ANUL_BORR = "", string I_LOTE = "" ,string I_FECH_CIERRE="", string I_UARIS_MOD = "")
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
                    I_ORDERID = string.IsNullOrEmpty(I_ORDERID) ? "" : I_ORDERID;
                    I_LIB = string.IsNullOrEmpty(I_LIB) ? "" : I_LIB;
                    I_CTEC = string.IsNullOrEmpty(I_CTEC) ? "" : I_CTEC;
                    I_LIB = string.IsNullOrEmpty(I_LIB) ? "" : I_REAP;
                    I_BLOQ = string.IsNullOrEmpty(I_BLOQ) ? "" : I_BLOQ;
                    I_DESBLOQ = string.IsNullOrEmpty(I_DESBLOQ) ? "" : I_DESBLOQ;
                    I_CERR = string.IsNullOrEmpty(I_CERR) ? "" : I_CERR;
                    I_BORR = string.IsNullOrEmpty(I_BORR) ? "" : I_BORR;
                    I_ANUL_BORR = string.IsNullOrEmpty(I_ANUL_BORR) ? "" : I_ANUL_BORR;
                    I_LOTE = string.IsNullOrEmpty(I_LOTE) ? "" : I_LOTE;
                    I_UARIS_MOD = string.IsNullOrEmpty(I_UARIS_MOD) ? "" : I_UARIS_MOD;
                    var result = await context.CallFunction("ZPP_FM_ACT_ESTADO_OF",
                        Input: f => f
                                        .SetField("I_ORDERID", I_ORDERID)
                                        .SetField("I_LIB", I_LIB)
                                        .SetField("I_CTEC", I_CTEC)
                                        .SetField("I_REAP", I_REAP)
                                        .SetField("I_BLOQ", I_BLOQ)
                                        .SetField("I_DESBLOQ", I_DESBLOQ)
                                        .SetField("I_CERR", I_CERR)
                                        .SetField("I_BORR", I_BORR)
                                        .SetField("I_ANUL_BORR", I_ANUL_BORR)
                                        .SetField("I_LOTE", I_LOTE)
                                         .SetField("I_FECH_CIERRE", string.IsNullOrWhiteSpace(I_FECH_CIERRE) ? new DateTime(1, 1, 1)  // equivalente a 0001-01-01
                                                                                                                                    : DateTime.ParseExact(I_FECH_CIERRE, "dd.MM.yyyy", null))
                                        .SetField("I_UARIS_MOD", I_UARIS_MOD),
                        Output: f => f
                            .MapTable("T_RETURN", s =>
                                 from TYPE in s.GetField<string>("TYPE")    // CHAR
                                 from ID in s.GetField<string>("ID")    // CHAR
                                 from NUMBER in s.GetField<string>("NUMBER")    // CHAR
                                 from MESSAGE in s.GetField<string>("MESSAGE")
                                 select new
                                 {
                                     TYPE,
                                     ID,
                                     NUMBER,
                                     MESSAGE
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