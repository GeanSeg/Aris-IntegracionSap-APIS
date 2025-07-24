using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad.PlantaAlimentoDespacho
{
        public class PlantaAlimentoDespachoEntity
        {
            public string BUDAT { get; set; }
            public string BLDAT { get; set; }
            public string BKTXT { get; set; }
            public string XBLNR { get; set; }
            public List<PlantaAlimentoDespachoDetalle> _PlantaAlimentoDespachoDetalle { get; set; }
        }

        public class PlantaAlimentoDespachoDetalle
    {
            public string MATNR { get; set; }
            public string WERKS { get; set; }
            public string LGORT { get; set; }
            public string CHARG { get; set; }
            public string UMMAT { get; set; }
            public string UMWRK { get; set; }
            public string UMLGO { get; set; }
            public string UMCHA { get; set; }
            public decimal ENTRY_QNT { get; set; }
        }

        public class PlantaAlimentoDespachoParameters
    {
            [SapName("I_CAB_MIGO")]
            public PlantaAlimentoDespachoParametersItem I_PLANTA_ALI_DES_CAB { get; set; }

            [SapName("IT_DET_MIGO")]
            public PlantaAlimentoDespachoParametersItems[] T_PLANTA_ALI_DES_DET { get; set; }
        }

        public class PlantaAlimentoDespachoResult
        {
            [SapName("E_MBLNR")]
            public string E_MBLNR { get; set; }

            [SapName("E_MJAHR")]
            public string E_MJAHR { get; set; }

            [SapName("ET_RETURN")]
            public PlantaAlimentoDespachoResultItems[] ET_RETURN { get; set; }
        }

        public class PlantaAlimentoDespachoParametersItem
    {
            [SapName("BUDAT")]
            public DateTime BUDAT { get; set; }

            [SapName("BLDAT")]
            public DateTime BLDAT { get; set; }

            [SapName("BKTXT")]
            public string BKTXT { get; set; }

            [SapName("XBLNR")]
            public string XBLNR { get; set; }

        }

        public class PlantaAlimentoDespachoParametersItems
    {
            [SapName("MATNR")]
            public string MATNR { get; set; }

            [SapName("WERKS")]
            public string WERKS { get; set; }

            [SapName("LGORT")]
            public string LGORT { get; set; }

            [SapName("CHARG")]
            public string CHARG { get; set; }

            [SapName("UMMAT")]
            public string UMMAT { get; set; }

            [SapName("UMWRK")]
            public string UMWRK { get; set; }

            [SapName("UMLGO")]
            public string UMLGO { get; set; }

            [SapName("UMCHA")]
            public string UMCHA { get; set; }

            [SapName("ENTRY_QNT")]
            public decimal ENTRY_QNT { get; set; }
        }

        public class PlantaAlimentoDespachoResultItems
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
