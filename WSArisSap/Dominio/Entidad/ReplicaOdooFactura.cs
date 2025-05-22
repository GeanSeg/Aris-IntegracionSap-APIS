using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad
{
    public class ReplicaOdooFactura
    {
        public string I_ENTREGA { get; set; }
    }

    public class FacturaParameters
    {
        [SapName("I_ENTREGA")]
        public string I_ENTREGA { get; set; }
    }

    public class FacturaResult
    {
        [SapName("E_FACTURA")]
        public string E_FACTURA { get; set; }

        [SapName("T_RETURN")]
        public FacturaResultItemTReturn[] T_RETURN { get; set; }
    }


    public class FacturaResultItemTReturn
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


}
