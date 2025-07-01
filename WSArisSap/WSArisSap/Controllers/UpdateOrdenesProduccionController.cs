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

        private string FormatOrden(string KnnrInput)
        {
            const int knnrLength = 12;

            // Si el valor es nulo o vacío, devolver cadena vacía
            if (string.IsNullOrWhiteSpace(KnnrInput))
            {
                return "";
            }

            // Limpiar espacios en blanco
            string trimmedKnnr = KnnrInput.Trim();

            // Verificar si el valor comienza con una letra
            if (char.IsLetter(trimmedKnnr[0]))
            {

                return trimmedKnnr;
            }

            // Verificar si el valor es numérico
            if (trimmedKnnr.All(char.IsDigit))
            {
                // Formatear con ceros a la izquierda hasta 18 caracteres
                string formattedKnnr = trimmedKnnr.PadLeft(knnrLength, '0');

                return formattedKnnr;
            }

            // Si no es numérico ni comienza con letra, devolver tal cual con log de advertencia

            return trimmedKnnr;
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
                                        .SetField("I_ORDERID", FormatOrden(I_ORDERID))
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
                         Output: f => (
                        from E_STATUS in f.GetField<string>("E_STATUS")
                        from E_ORDERID in f.GetField<string>("E_ORDERID")
                        from T_RETURN in f.MapTable("T_RETURN", r =>
                                    from TYPE in r.GetField<string>("TYPE")
                                    from ID in r.GetField<string>("ID")
                                    from NUMBER in r.GetField<string>("NUMBER")
                                    from MESSAGE in r.GetField<string>("MESSAGE")
                                    from LOG_NO in r.GetField<string>("LOG_NO")
                                    from LOG_MSG_NO in r.GetField<decimal>("LOG_MSG_NO")
                                    from MESSAGE_V1 in r.GetField<string>("MESSAGE_V1")
                                    from MESSAGE_V2 in r.GetField<string>("MESSAGE_V2")
                                    from MESSAGE_V3 in r.GetField<string>("MESSAGE_V3")
                                    from MESSAGE_V4 in r.GetField<string>("MESSAGE_V4")
                                    from PARAMETER in r.GetField<string>("PARAMETER")
                                    from ROW in r.GetField<string>("ROW")
                                    from FIELD in r.GetField<string>("FIELD")
                                    from SYSTEM in r.GetField<string>("SYSTEM")
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
                            E_STATUS,
                            E_ORDERID,
                            T_RETURN
                        }
                    )
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