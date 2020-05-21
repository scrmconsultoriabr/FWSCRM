using System;
using FWSCRM.BD;

namespace wfaFWSCRM
{
    public class clsDTO : PersistClass
    {
        public Int32 pms_cod_empresa { get; set; }

        public String pms_cod_grupo { get; set; }

        public String pms_cod_parametro { get; set; }

        public String pms_des_parametro { get; set; }

        public String pms_val_parametro { get; set; }

    }
}
