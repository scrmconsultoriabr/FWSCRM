using FWSCRM.BD;
using System;
using System.Linq;

namespace wfaFWSCRM
{
    public class clsBanco : PersistClass
    {
        public string ObtemStringConexao()
        {
            return ObterParametros(1, "sis", "FormatoDt").pms_val_parametro;
        }
        public clsDTO ObterParametros(int PEmpresa, String PGrupo, String PParametro)
        {
            return getDataReaderList<clsDTO>("select * from parametros where pms_cod_empresa = " + PEmpresa.ToString() + " and pms_cod_grupo = '" + PGrupo + "' and pms_cod_parametro = '" + PParametro + "' ").FirstOrDefault();
        }
    }
}
