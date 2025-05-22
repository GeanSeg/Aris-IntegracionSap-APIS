using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad.GestionPedidosCobranzasAris
{
    public class CobranzaAnticCompAris
    {
        public string BLDAT { get; set; }

        public string BUDAT { get; set; }

        public string BLART { get; set; }

        public string BUKRS { get; set; }

        public string WAERS { get; set; }

        public string REFERENCIA { get; set; }

        public string NRO_FACTURA { get; set; }

        public string BKTXT { get; set; }

        public string NEWKO { get; set; }

        public List<CobranzaAnticCompArisDetalle> cobranzaAnticCompArisDetalles { get; set; } 

    }
    public class CobranzaAnticCompArisDetalle
    {
         public string REF_ANTICIPO { get; set; }
    }
        
    public class CobranzaAnticCompArisParameters
    {
        [SapName("I_COBRANZA_ANT")]
        public CobranzaAnticCompArisParametersItem I_COBRANZA_ANT { get; set; }

        [SapName("T_COBRANZA_DET_ANT")]
        public CobranzaAnticCompArisParametersItems[] T_COBRANZA_DET_ANT { get; set; }
    }

    public class CobranzaAnticCompArisResult
    {
        [SapName("E_BELNR")]
        public string E_BELNR { get; set; }

        [SapName("T_RETURN")]
        public CobranzaAnticCompArisResultItems[] T_RETURN { get; set; }
    }

    public class CobranzaAnticCompArisParametersItem
    {
        [SapName("BLDAT")]
        public DateTime BLDAT { get; set; }

        [SapName("BUDAT")]
        public DateTime BUDAT { get; set; }

        [SapName("BLART")]
        public string BLART { get; set; }

        [SapName("BUKRS")]
        public string BUKRS { get; set; }

        [SapName("WAERS")]
        public string WAERS { get; set; }

        [SapName("REFERENCIA")]
        public string REFERENCIA { get; set; }

        [SapName("NRO_FACTURA")]
        public string NRO_FACTURA { get; set; }

        [SapName("BKTXT")]
        public string BKTXT { get; set; }

        [SapName("NEWKO")]
        public string NEWKO { get; set; }

    }

    public class CobranzaAnticCompArisParametersItems
    {
        [SapName("REF_ANTICIPO")]
        public string REF_ANTICIPO { get; set; }
    }

    public class CobranzaAnticCompArisResultItems
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

