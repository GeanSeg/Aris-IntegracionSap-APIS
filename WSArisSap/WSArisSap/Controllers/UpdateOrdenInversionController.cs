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
        public async Task<IActionResult> GetUpdateOrdenInversion( string I_CTEC="",string I_ANUL="",string I_CERR="", string I_REAP="", string I_ORDERID="", string I_FEC_CTEC="")
        {
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

                    I_CTEC = string.IsNullOrEmpty(I_CTEC) ? "" : I_CTEC;
                    I_ANUL = string.IsNullOrEmpty(I_ANUL) ? "" : I_ANUL;
                    I_CERR = string.IsNullOrEmpty(I_CERR) ? "" : I_CERR;
                    I_REAP = string.IsNullOrEmpty(I_REAP) ? "" : I_REAP;
                    I_ORDERID = string.IsNullOrEmpty(I_ORDERID) ? "" : I_ORDERID;
                    I_FEC_CTEC = string.IsNullOrEmpty(I_FEC_CTEC) ? "" : I_FEC_CTEC;
                    var result = await context.CallFunction("ZCO_FM_UPDATE_STATUS_ORDEN_INV",
                        Input: f => f
                                        .SetField("I_CTEC", I_CTEC)
                                        .SetField("I_CERR", I_CERR)
                                        .SetField("I_REAP", I_REAP)
                                        .SetField("I_ORDERID", I_ORDERID)
                                        .SetField("I_FEC_CTEC", string.IsNullOrEmpty(I_FEC_CTEC) ? default(DateTime) : DateTime.ParseExact(I_FEC_CTEC, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture))
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