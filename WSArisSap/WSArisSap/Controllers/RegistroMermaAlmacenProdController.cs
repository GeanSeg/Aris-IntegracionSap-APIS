using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("api")]
    public class RegistroMermaAlmacenProdController : ControllerBase
    {
        public class NotConsOiItems
        {
            public string MATNR { get; set; }
            public string WERKS { get; set; }
            public string LGORT { get; set; }
            public string CHARG { get; set; }
            public string ENTRY_QNT { get; set; }
            public string COST_CENTER { get; set; }
        }

        public class RegistroMermaRequest
        {
            public string I_MIGO_PED_IMP { get; set; }
            public string I_MIGO_MERMAS { get; set; }
            public string BUDAT { get; set; }
            public string BLDAT { get; set; }
            public string EBELN { get; set; }
            public string UARIS_CREA { get; set; }
            public string UARIS_MOD { get; set; }
            public List<NotConsOiItems> Items { get; set; }
        }

        private readonly IConfiguration _configuration;

        public RegistroMermaAlmacenProdController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("RegistroMermaAlmacenProdController")]
        public async Task<IActionResult> CreateRegistroMermaAlmacenProd([FromBody] RegistroMermaRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { Error = "El cuerpo de la solicitud no puede estar vacío." });
            }

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
                {"passwd", "Rocio*25"},
                {"lang", "ES"}
            };

            var connectionBuilder = new ConnectionBuilder(settings);
            var connFunc = connectionBuilder.Build();

            using (var context = new RfcContext(connFunc))
            {
                try
                {
                    string iMigoPedImp = string.IsNullOrEmpty(request.I_MIGO_PED_IMP) ? "" : request.I_MIGO_PED_IMP;
                    string iMigoMermas = string.IsNullOrEmpty(request.I_MIGO_MERMAS) ? "" : request.I_MIGO_MERMAS;
                    string budat = string.IsNullOrEmpty(request.BUDAT) ? "" : request.BUDAT;
                    string bldat = string.IsNullOrEmpty(request.BLDAT) ? "" : request.BLDAT;
                    string ebeln = string.IsNullOrEmpty(request.EBELN) ? "" : request.EBELN;
                    string uarisCrea = string.IsNullOrEmpty(request.UARIS_CREA) ? "" : request.UARIS_CREA;
                    string uarisMod = string.IsNullOrEmpty(request.UARIS_MOD) ? "" : request.UARIS_MOD;

                    if (request.Items == null || !request.Items.Any())
                    {
                        return BadRequest(new { Error = "La lista de ítems no puede estar vacía." });
                    }

                    var result = await context.CallFunction("ZMM_FM_GEN_MIGO_PED",
                        Input: f => f
                            .SetField("I_MIGO_PED_IMP", iMigoPedImp)
                            .SetField("I_MIGO_MERMAS", iMigoMermas)
                            .SetStructure("ES_CABECERA_MIGO", s => s
                                .SetField("BUDAT", DateTime.ParseExact(budat, "dd.MM.yyyy", null))
                                .SetField("BLDAT", DateTime.ParseExact(bldat, "dd.MM.yyyy", null))
                                .SetField("EBELN", ebeln)
                                .SetField("UARIS_CREA", uarisCrea)
                                .SetField("UARIS_MOD", uarisMod))
                            .SetTable("T_DETALLE_MIGO", request.Items,
                                (structure, item) => structure
                                    .SetField("MATNR", item.MATNR)
                                    .SetField("WERKS", item.WERKS)
                                    .SetField("LGORT", item.LGORT)
                                    .SetField("CHARG", item.CHARG)
                                    .SetField("ENTRY_QNT", item.ENTRY_QNT)
                                    .SetField("COST_CENTER", item.COST_CENTER)
                                    
                            ),
                        Output: f => (
                            from E_DOC_MATNR in f.GetField<string>("E_DOC_MATNR")
                            from T_DETALLE_MIGO in f.MapTable("T_DETALLE_MIGO", s =>
                                from MATNR in s.GetField<string>("MATNR")
                                from WERKS in s.GetField<string>("WERKS")
                                from LGORT in s.GetField<string>("LGORT")
                                from CHARG in s.GetField<string>("CHARG")
                                from ENTRY_QNT in s.GetField<string>("ENTRY_QNT")
                                from COST_CENTER in s.GetField<string>("COST_CENTER")
                                select new
                                {
                                    MATNR,
                                    WERKS,
                                    LGORT,
                                    CHARG,
                                    ENTRY_QNT,
                                    COST_CENTER
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
                                E_DOC_MATNR,
                                T_DETALLE_MIGO,
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