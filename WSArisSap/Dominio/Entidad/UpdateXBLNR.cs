using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad.UpdateXBLNR
{
    public class UpdateXBLNR
    {
        public string I_VBELN { get; set; } 
        public string I_XBLNR { get; set; } 
    }

    public class UpdateXBLNR_Parameters
    {
        [SapName("I_VBELN")]
        public string? I_VBELN { get; set; }

        [SapName("I_XBLNR")]
        public string? I_XBLNR { get; set; }
    }

    public class UpdateXBLNR_Result 
    {
        [SapName("T_RETURN")]
        public UpdateXBLNR_ResultItems[] T_RETURN { get; set; }
    }

    public class UpdateXBLNR_ResultItems
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
