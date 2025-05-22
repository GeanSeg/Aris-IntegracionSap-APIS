using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad
{
    public class CreateOrdenInversion
    {

        public class CreateOrdenInversionParameters
        {
            [SapName("IS_DAT_ORDEN")]
            public CreateOrdenInversionResultItemDAT DAT { get; set; }
        }

        public class CreateOrdenInversionResult
        {
            [SapName("E_ORDERID")]
            public string E_ORDERID { get; set; }

            [SapName("T_RETURN")]
            public CreateOrdenInversionTReturn[] T_RETURN { get; set; }
        }

        public class CreateOrdenInversionResultItemDAT
        {
            [SapName("CO_AREA")]
            public string CO_AREA { get; set; }

            [SapName("COMP_CODE")]
            public string COMP_CODE { get; set; }

            [SapName("ORDER_TYPE")]
            public string ORDER_TYPE { get; set; }

            [SapName("ORDER")]
            public string ORDER { get; set; }

            [SapName("FUNC_AREA_LONG")]
            public string FUNC_AREA_LONG { get; set; }

            [SapName("OBJECTCLASS")]
            public string OBJECTCLASS { get; set; }

            [SapName("PROFIT_CTR")]
            public string PROFIT_CTR { get; set; }

            [SapName("REQU_COMP_CODE")]
            public string REQU_COMP_CODE { get; set; }

            [SapName("INVEST_PROFILE")]
            public string INVEST_PROFILE { get; set; }

            [SapName("CURRENCY")]
            public string CURRENCY { get; set; }
        }

        public class CreateOrdenInversionTReturn
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
}
