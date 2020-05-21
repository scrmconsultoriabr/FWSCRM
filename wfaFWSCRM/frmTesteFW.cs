using FWSCRM.Util;
using System;
using System.Windows.Forms;

namespace wfaFWSCRM
{
    public partial class frmTesteFW : Form
    {
        private bool AtivaDesativaEmpresa { get; set; } = false;
        public frmTesteFW()
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
            rbnDesativada.Checked = false;
            rbnAtivada.Checked = false;
            try
            {
                rbnAtivada.Checked = Geral.Instance.VerificaSituacaoEmpresa(mtbCNPJ.Text, tbxUrlWebService.Text, 5000);

                rbnDesativada.Checked = !rbnAtivada.Checked;

                btnAtivaInativaEmpresa.Text = "Ativar";
                if (rbnAtivada.Checked)
                {
                    btnAtivaInativaEmpresa.Text = "Desativar";
                }

                if (!AtivaDesativaEmpresa)
                {
                    MessageBox.Show("Consulta concluída com sucesso!");
                }
                AtivaDesativaEmpresa = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao consultar situação da empresa !" + Environment.NewLine + ex.Message);
            }
                
        }

       private void btnAtivaInativaEmpresa_Click(object sender, EventArgs e)
        {
            AtivaDesativaEmpresa = true;
            string l_Acao = "I";
            string l_Mensagem = "Empresa Ativada com Sucesso !";
            if (rbnDesativada.Checked)
            {
                l_Acao = "A";
            }
            try
            {
                bool l_Retorno = Geral.Instance.AtivaDesativaEmpresa(tbxUrlWebService.Text, 5000, mtbCNPJ.Text, l_Acao);
                
                btnStatus.PerformClick();

                if ((l_Retorno) && (l_Acao == "I"))
                {
                    l_Mensagem = "Empresa Desativada com Sucesso !";
                }

            }
            catch (Exception ex)
            {
                l_Mensagem = "Erro ao tentar Ativar empresa" + Environment.NewLine + ex.Message;
                if (l_Acao == "I")
                {
                    l_Mensagem = "Erro ao tentar Inativar empresa";
                }

            }
            MessageBox.Show(l_Mensagem);
        }

        private void btnConexao_Click(object sender, EventArgs e)
        {
            clsBanco objbanco = new clsBanco();
            tbxConexao.Text = objbanco.ObtemStringConexao();
        }
    }
}
