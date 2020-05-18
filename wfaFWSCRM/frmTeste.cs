using FWSCRM.Util;
using System;
using System.Windows.Forms;

namespace wfaFWSCRM
{
    public partial class frmTeste : Form
    {
        public frmTeste()
        {
            InitializeComponent();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStatus_Click(object sender, EventArgs e)
        {
            /*
             
             15101228000145 ==> I
             15152018000186 ==> A

           */

            rbnAtivada.Checked = Geral.Instance.VerificaSituacaoEmpresa(mtbCNPJ.Text, tbxUrlWebService.Text, 5000);
           rbnDesativada.Checked = !rbnAtivada.Checked;
        }

        
    }
}
