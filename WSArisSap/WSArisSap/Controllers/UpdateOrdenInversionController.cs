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
using System.Runtime.InteropServices;


namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("api/")]
    public class UpdateOrdenInversionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UpdateOrdenInversionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("UpdateOrdenInversionController")]
        public async Task<IActionResult> GetUpdateOrdenInversion(string I_ORDERID, string I_FEC_CTEC, string I_CTEC="",string I_ANUL="",string I_CERR="", string I_REAP="")
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
                {"lang", "EN"}
            };

            var connectionBuilder = new ConnectionBuilder(settings);
            var connFunc = connectionBuilder.Build();

            using (var context = new RfcContext(connFunc))
            {
                try
                {

                    I_CTEC = string.IsNullOrEmpty(I_CTEC) ? "" : I_CTEC;
                    I_ANUL = string.IsNullOrEmpty(I_ANUL) ? "" : I_ANUL;
                    I_CERR = string.IsNullOrEmpty(I_CERR) ? "" : I_CERR;
                    I_REAP = string.IsNullOrEmpty(I_REAP) ? "" : I_REAP;

                    var result = await context.CallFunction("ZCO_FM_UPDATE_STATUS_ORDEN_INV",
                        Input: f => f
                                        .SetField("I_CTEC", I_CTEC)
                                        .SetField("I_CERR", I_CERR)
                                        .SetField("I_REAP", I_REAP)
                                        .SetField("I_ORDERID", I_ORDERID)
                                        .SetField("I_FEC_CTEC", DateTime.ParseExact(I_FEC_CTEC, "dd.MM.yyyy", null))
                                        .SetField("I_ANUL", I_ANUL),
                                   
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