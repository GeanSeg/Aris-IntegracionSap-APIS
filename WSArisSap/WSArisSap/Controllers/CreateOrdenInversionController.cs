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
using Application.Interfaces;
using Domain.Entidad;
using SapNwRfc;
using static Domain.Entidad.CreateOrdenInversion;



namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("api/")]
    public sealed class CreateOrdenInversionController (
        ILibraryInitializer libraryInitializer,
        IInitializerContextSAP initializerContextSAP
        ) : ControllerBase
    {

        [HttpGet("CreateOrdenInversionController")]
        public async Task<IActionResult> GetCreateOrdenInversion(string CO_AREA, string COMP_CODE, string ORDER_TYPE,string ORDER, string FUNC_AREA_LONG, string PROFIT_CTR,string REQU_COMP_CODE, string INVEST_PROFILE,string CURRENCY,string OBJECTCLASS)
        {
            try
            {
                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer)
                {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();
                OBJECTCLASS = string.IsNullOrWhiteSpace(OBJECTCLASS) ? "" : OBJECTCLASS;

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                using var someFunction = connection.CreateFunction("ZCO_FM_CREATE_ORDEN_INV");

                var result = someFunction.Invoke<CreateOrdenInversionResult>(new CreateOrdenInversionParameters
                {
                    DAT = new CreateOrdenInversionResultItemDAT
                    {
                        CO_AREA = CO_AREA,
                        COMP_CODE = COMP_CODE,
                        ORDER_TYPE =  ORDER_TYPE,
                        ORDER = ORDER ,
                        FUNC_AREA_LONG = FUNC_AREA_LONG,
                        OBJECTCLASS = OBJECTCLASS == "INVER" ? "IV" : OBJECTCLASS,
                        PROFIT_CTR = PROFIT_CTR,
                        REQU_COMP_CODE = REQU_COMP_CODE,
                        INVEST_PROFILE = INVEST_PROFILE,
                        CURRENCY = CURRENCY

                    },
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}

