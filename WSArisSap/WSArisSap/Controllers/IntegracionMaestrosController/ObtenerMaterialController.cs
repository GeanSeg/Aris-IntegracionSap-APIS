using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WSArisSap.Controllers.IntegracionMaestrosController
{
    [ApiController]
    [Route("api/")]
    public class ObtenerMaterialController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ObtenerMaterialController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("ObtenerMaterialController")]
        public async Task<IActionResult> GetObtenerMaterial(
         string? I_BUKRS = null, 
         string? I_MTART = null,
         string? I_MATKL = null,
         string I_WERKS = "",
         string? I_LGORT = null,
         string? I_MATNR = null,
         string? I_MAKTX = null,
         string? I_XCHPF = null,
         string? I_FECCREA_I = null,
         string? I_FECCREA_F = null,
         string? I_FECMOD_I = null,
         string? I_FECMOD_F = null)
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
                    var result = await context.CallFunction("ZMM_FM_MIGR_MATERIAL_DATA",
                        Input: f =>
                        {
                            
                            f.SetField("I_BUKRS", I_BUKRS ?? "");
                            f.SetField("I_MTART", I_MTART ?? ""); 
                            f.SetField("I_MATKL", I_MATKL ?? "");
                            if (!string.IsNullOrEmpty(I_WERKS))
                            {
                                f.SetTable("I_WERKS", new string[] { I_WERKS },
                                    (structure, werksValue) => structure
                                        .SetField("WERKS", werksValue));
                            }
                            f.SetField("I_LGORT", I_LGORT ?? "");
                            f.SetField("I_MATNR", I_MATNR ?? "");
                            f.SetField("I_MAKTX", I_MAKTX ?? "");
                            f.SetField("I_XCHPF", I_XCHPF ?? "");
                            DateTime? ParseDate(string? dateStr)
                            {
                                return string.IsNullOrWhiteSpace(dateStr)
                                    ? null
                                    : DateTime.ParseExact(dateStr, "dd.MM.yyyy", null);
                            }

                            var fechaCreaI = ParseDate(I_FECCREA_I);
                            var fechaCreaF = ParseDate(I_FECCREA_F);
                            var fechaModI = ParseDate(I_FECMOD_I);
                            var fechaModF = ParseDate(I_FECMOD_F);

                            if (fechaCreaI != null) f.SetField("I_FECCREA_I", fechaCreaI.Value);
                            if (fechaCreaF != null) f.SetField("I_FECCREA_F", fechaCreaF.Value);
                            if (fechaModI != null) f.SetField("I_FECMOD_I", fechaModI.Value);
                            if (fechaModF != null) f.SetField("I_FECMOD_F", fechaModF.Value);

                            return f;
                        },
                        Output: f => (
                            from T_DATA_MAT in f.MapTable("T_DATA_MAT", s =>
                                from MATNR in s.GetField<string>("MATNR")
                                from MAKTX in s.GetField<string>("MAKTX")
                                from WERKS in s.GetField<string>("WERKS")
                                from LGORT in s.GetField<string>("LGORT")
                                from MEINS in s.GetField<string>("MEINS")
                                from GEWEI in s.GetField<string>("GEWEI")
                                from MENGE in s.GetField<string>("MENGE")
                                from MATKL in s.GetField<string>("MATKL")
                                from MTART in s.GetField<string>("MTART")
                                from SPART in s.GetField<string>("SPART")
                                from XCHPF in s.GetField<string>("XCHPF")
                                from XCHPF_C in s.GetField<string>("XCHPF_C")
                                from PRICE_UN in s.GetField<string>("PRICE_UN")
                                from WAERS in s.GetField<string>("WAERS")
                                from PRCTR in s.GetField<string>("PRCTR")

                                select new
                                {
                                    MATNR,
                                    MAKTX,
                                    WERKS,
                                    LGORT,
                                    MEINS = MEINS == "ST" ? "UN" : MEINS,
                                    GEWEI,
                                    MENGE,
                                    MATKL,
                                    MTART,
                                    SPART,
                                    XCHPF,
                                    XCHPF_C,
                                    PRICE_UN,
                                    WAERS,
                                    PRCTR
                                })
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
                                T_DATA_MAT,
                                T_RETURN
                            }
                        )
                    );

                    return Ok(new { Data = result.Case });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Error = ex.Message });
                }
            }
        }

    }
}
