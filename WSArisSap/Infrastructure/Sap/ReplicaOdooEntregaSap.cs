using Application.Interfaces;
using Dbosoft.YaNco;
using Domain.Entidad;
using Dominio.Entidad;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Sap
{
    public class ReplicaOdooEntregaSap(ILibraryInitializer libraryInitializer,IInitializerContextSAP initializerContextSAP ): IEntregaService
     {
        public async Task<bool> CrearEntregaSAP(ReplicaOdooEntrega replicaOdooEntrega)
        {
            try
            {
                //libraryInitializer.InitializeLibrary();
                //using var context = initializerContextSAP.InitializeContextConnSap();

                //var resultCabecera = await context.CallFunction("ZSD_REPLICA_ODOO_ENTREGA",
                //    Input: f => f.SetStructure("ES_CAB_DESPA", s => s
                //                    .SetField("VSTEL", replicaOdooEntrega.VSTEL)
                //                    .SetField("DATBI", replicaOdooEntrega.DATBI)
                //                    .SetField("VBELN", replicaOdooEntrega.VBELN)
                //                    .SetField("WADAT_IST", replicaOdooEntrega.WADAT_IST)
                //                    .SetTable("T_DET_DESP", replicaOdooEntrega.replicaOdooEntregaDetalle,
                //                             (structure, detalle) => structure
                //                            .SetField("POSNR", detalle.POSNR)
                //                            .SetField("KWMENG", detalle.KWMENG)
                //                            //.SetField("VRKME", detalle.VRKME)
                //                            .SetField("CHARG", detalle.CHARG)
                //                        )),
                //      Output: f => (
                //        from E_VBELN in f.GetField<string>("E_VBELN")
                //        from T_RETURN in f.MapTable("T_RETURN", s =>
                //        from TYPE in s.GetField<string>("TYPE")
                //        from ID in s.GetField<string>("ID")
                //        from NUMBER in s.GetField<string>("NUMBER")
                //        from MESSAGE in s.GetField<string>("MESSAGE")
                //        select new
                //        {
                //            TYPE,
                //            ID,
                //            NUMBER,
                //            MESSAGE
                //        })
                //        select new
                //        {
                //            E_VBELN,
                //            T_RETURN
                //        }));

                //var result = resultCabecera.Match(
                //    right =>
                //    {
                //        var eVbeln = right.E_VBELN;
                //        var tReturn = right.T_RETURN;

                //        return (eVbeln, tReturn);
                //    },
                //    left =>
                //    {
                //        return (null, null);
                //    }
                //);

                //var eVbeln = result.Item1;
                //var tReturn = result.Item2;

                //if (string.IsNullOrEmpty(eVbeln))
                //{

                //}else if(tReturn == null || tReturn.Any())
                //{

                //}else
                //{

                //}

            }
            catch (Exception ex)
            {

            }
            return true;
        }
    }
}
