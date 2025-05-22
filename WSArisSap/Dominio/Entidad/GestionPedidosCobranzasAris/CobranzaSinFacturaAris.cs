using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad.GestionPedidosCobranzasAris
{
    public class CobranzaSinFacturaAris
    {
        public string BLDAT { get; set; }
        public string BUDAT { get; set; }
        public string BLART { get; set; }
        public string BUKRS { get; set; }
        public string WAERS { get; set; }
        public string XBLNR { get; set; } = "";
        public string BKTXT { get; set; }   
        public string NEWKO { get; set; }   
        public string UMSKZ { get; set; }
        public string KONTO { get; set; }
        public decimal WRBTR { get; set; }
        public string VALUT {  get; set; }
        public string SGTXT { get; set; } = "";
        public string ZUONR { get; set; }
        public string MWSKZ { get; set; }
    }

    public class CobranzaSinFacturaArisParameters
    {
        [SapName("I_COBRANZA_SF_CAB")]
        public CobranzaSinFacturaArisParametersItem I_COBRANZA_CAB { get; set; }
    }

    public class CobranzaSinFacturaArisResult
    {
        [SapName("E_BELNR")]
        public string E_BELNR { get; set; }

        [SapName("T_RETURN")]
        public CobranzaSinFacturaArisResultItems[] T_RETURN { get; set; }

    }

    public class CobranzaSinFacturaArisParametersItem
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

        [SapName("XBLNR")]
        public string XBLNR { get; set; }

        [SapName("BKTXT")]
        public string BKTXT { get; set; }

        [SapName("NEWKO")]
        public string NEWKO { get; set; }

        [SapName("UMSKZ")]
        public string UMSKZ { get; set; }

        [SapName("KONTO")]
        public string KONTO { get; set; }

        [SapName("WRBTR")]
        public decimal WRBTR { get; set; }

        [SapName("VALUT")]
        public DateTime VALUT { get; set; }

        [SapName("SGTXT")]
        public string SGTXT { get; set; }

        [SapName("ZUONR")]
        public string ZUONR { get; set; }

        [SapName("MWSKZ")]
        public string MWSKZ { get; set; }

    }

    public class CobranzaSinFacturaArisResultItems
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
