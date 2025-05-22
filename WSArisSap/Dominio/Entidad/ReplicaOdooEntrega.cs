using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad
{
    public class ReplicaOdooEntrega
    {
        public string VSTEL { get; set; }
        public string DATBI { get; set; }
        public string VBELN { get; set; }
        public string WADAT_IST { get; set; }
        public List<ReplicaOdooEntregaDetalle> replicaOdooEntregaDetalle { get; set; }
    }

   public class ReplicaOdooEntregaDetalle
    {
        public string POSNR { get; set; }
        public decimal KWMENG { get; set; }
        public string VRKME { get; set; }
        public string CHARG { get; set; }
    }

    public class EntregaParameters
    {
        [SapName("ES_CAB_DESPA")]
        public EntregaResultItemCAB CAB { get; set; }

        [SapName("T_DET_DESP")]
        public EntregaResultItemDET[] DET { get; set; }
    }

   public class EntregaResult
    {
        [SapName("E_VBELN_ENT")]
        public string E_VBELN_ENT { get; set; }

        [SapName("T_RETURN")]
        public EntregaResultItemTReturn[] T_RETURN { get; set; }
    }
    public class EntregaResultItemTReturn
    {
        [SapName("TYPE")]
        public string TYPE { get; set; }

        [SapName("ID")]
        public string ID { get; set; }

        [SapName("NUMBER")]
        public string NUMBER { get; set; }

        [SapName("MESSAGE")]
        public string MESSAGE { get; set; }
    }

    public class EntregaResultItemCAB
    {
        [SapName("VSTEL")]
        public string VSTEL { get; set; }

        [SapName("DATBI")]
        public DateTime DATBI { get; set; }

        [SapName("VBELN")]
        public string VBELN { get; set; }

        [SapName("WADAT_IST")]
        public DateTime WADAT_IST { get; set; }
    }

    public class EntregaResultItemDET
    {
        [SapName("POSNR")]
        public string POSNR { get; set; }

        [SapName("KWMENG")]
        public decimal KWMENG { get; set; }

        [SapName("VRKME")]
        public string VRKME { get; set; }

        [SapName("CHARG")]
        public string CHARG { get; set; }
    }

}
