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
    public class RegistrarEncasetamientoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistrarEncasetamientoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("RegistrarEncasetamientoController")]
        public async Task<IActionResult> GetObtenerOrdenesCompra(string E_PEDIDO)
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

                

                    var result = await context.CallFunction("ZMM_FM_CONSULT_PED",
                        Input: f => f
                                      
                                        .SetField("E_PEDIDO", E_PEDIDO),


                        Output: f => f
                            .MapStructure("ES_CABECERA", s =>
                                 from BSART in s.GetField<string>("BSART")    // CHAR
                                 from LIFNR in s.GetField<string>("LIFNR")
                                 from RESWK in s.GetField<string>("RESWK")
                                 from BEDAT in s.GetField<string>("BEDAT")
                                 from EKORG in s.GetField<string>("EKORG")
                                 from EKGRP in s.GetField<string>("EKGRP")
                                 from BUKRS in s.GetField<string>("BUKRS")
                                 from AEDAT in s.GetField<string>("AEDAT")
                                 from ZZ_LUG_ENTR in s.GetField<string>("ZZ_LUG_ENTR")
                                 from UARIS_CREA in s.GetField<string>("UARIS_CREA")
                                 from UARIS_MOD in s.GetField<string>("UARIS_MOD")
                           
                                 select new
                                 {
                                     BSART,
                                     LIFNR,
                                     RESWK,
                                     BEDAT,
                                     EKORG,
                                     EKGRP,
                                     BUKRS,
                                     AEDAT,
                                     ZZ_LUG_ENTR,
                                     UARIS_CREA,
                                     UARIS_MOD
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