namespace wfaFWSCRM
{
    partial class frmTeste
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbcTestaFWSCRM = new System.Windows.Forms.TabControl();
            this.tpeWebService = new System.Windows.Forms.TabPage();
            this.tbxUrlWebService = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxAtivaDesativaEmpresa = new System.Windows.Forms.CheckBox();
            this.gbxSituacaoEmpresa = new System.Windows.Forms.GroupBox();
            this.rbnDesativada = new System.Windows.Forms.RadioButton();
            this.rbnAtivada = new System.Windows.Forms.RadioButton();
            this.btnStatus = new System.Windows.Forms.Button();
            this.mtbCNPJ = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pnlTestaFWSCRM = new System.Windows.Forms.Panel();
            this.btnSair = new System.Windows.Forms.Button();
            this.tbcTestaFWSCRM.SuspendLayout();
            this.tpeWebService.SuspendLayout();
            this.gbxSituacaoEmpresa.SuspendLayout();
            this.pnlTestaFWSCRM.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcTestaFWSCRM
            // 
            this.tbcTestaFWSCRM.Controls.Add(this.tpeWebService);
            this.tbcTestaFWSCRM.Controls.Add(this.tabPage2);
            this.tbcTestaFWSCRM.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbcTestaFWSCRM.Location = new System.Drawing.Point(0, 0);
            this.tbcTestaFWSCRM.Name = "tbcTestaFWSCRM";
            this.tbcTestaFWSCRM.SelectedIndex = 0;
            this.tbcTestaFWSCRM.Size = new System.Drawing.Size(299, 185);
            this.tbcTestaFWSCRM.TabIndex = 0;
            // 
            // tpeWebService
            // 
            this.tpeWebService.AutoScroll = true;
            this.tpeWebService.Controls.Add(this.tbxUrlWebService);
            this.tpeWebService.Controls.Add(this.label2);
            this.tpeWebService.Controls.Add(this.cbxAtivaDesativaEmpresa);
            this.tpeWebService.Controls.Add(this.gbxSituacaoEmpresa);
            this.tpeWebService.Controls.Add(this.btnStatus);
            this.tpeWebService.Controls.Add(this.mtbCNPJ);
            this.tpeWebService.Controls.Add(this.label1);
            this.tpeWebService.Location = new System.Drawing.Point(4, 22);
            this.tpeWebService.Name = "tpeWebService";
            this.tpeWebService.Padding = new System.Windows.Forms.Padding(3);
            this.tpeWebService.Size = new System.Drawing.Size(291, 159);
            this.tpeWebService.TabIndex = 0;
            this.tpeWebService.Text = "Web Service";
            this.tpeWebService.UseVisualStyleBackColor = true;
            // 
            // tbxUrlWebService
            // 
            this.tbxUrlWebService.Location = new System.Drawing.Point(11, 19);
            this.tbxUrlWebService.Name = "tbxUrlWebService";
            this.tbxUrlWebService.Size = new System.Drawing.Size(273, 20);
            this.tbxUrlWebService.TabIndex = 6;
            this.tbxUrlWebService.Text = "http://192.168.15.9/PortalProjetos/PortalProjetos.asmx";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "URL WebService";
            // 
            // cbxAtivaDesativaEmpresa
            // 
            this.cbxAtivaDesativaEmpresa.AutoSize = true;
            this.cbxAtivaDesativaEmpresa.Location = new System.Drawing.Point(11, 135);
            this.cbxAtivaDesativaEmpresa.Name = "cbxAtivaDesativaEmpresa";
            this.cbxAtivaDesativaEmpresa.Size = new System.Drawing.Size(109, 17);
            this.cbxAtivaDesativaEmpresa.TabIndex = 4;
            this.cbxAtivaDesativaEmpresa.Text = "Ativar / Desativar";
            this.cbxAtivaDesativaEmpresa.UseVisualStyleBackColor = true;
            // 
            // gbxSituacaoEmpresa
            // 
            this.gbxSituacaoEmpresa.Controls.Add(this.rbnDesativada);
            this.gbxSituacaoEmpresa.Controls.Add(this.rbnAtivada);
            this.gbxSituacaoEmpresa.Location = new System.Drawing.Point(11, 82);
            this.gbxSituacaoEmpresa.Name = "gbxSituacaoEmpresa";
            this.gbxSituacaoEmpresa.Size = new System.Drawing.Size(207, 47);
            this.gbxSituacaoEmpresa.TabIndex = 3;
            this.gbxSituacaoEmpresa.TabStop = false;
            this.gbxSituacaoEmpresa.Text = "Situação Empresa";
            // 
            // rbnDesativada
            // 
            this.rbnDesativada.AutoSize = true;
            this.rbnDesativada.Enabled = false;
            this.rbnDesativada.Location = new System.Drawing.Point(83, 20);
            this.rbnDesativada.Name = "rbnDesativada";
            this.rbnDesativada.Size = new System.Drawing.Size(79, 17);
            this.rbnDesativada.TabIndex = 2;
            this.rbnDesativada.TabStop = true;
            this.rbnDesativada.Text = "Desativada";
            this.rbnDesativada.UseVisualStyleBackColor = true;
            // 
            // rbnAtivada
            // 
            this.rbnAtivada.AutoSize = true;
            this.rbnAtivada.Enabled = false;
            this.rbnAtivada.Location = new System.Drawing.Point(6, 20);
            this.rbnAtivada.Name = "rbnAtivada";
            this.rbnAtivada.Size = new System.Drawing.Size(61, 17);
            this.rbnAtivada.TabIndex = 1;
            this.rbnAtivada.TabStop = true;
            this.rbnAtivada.Text = "Ativada";
            this.rbnAtivada.UseVisualStyleBackColor = true;
            // 
            // btnStatus
            // 
            this.btnStatus.Location = new System.Drawing.Point(143, 56);
            this.btnStatus.Name = "btnStatus";
            this.btnStatus.Size = new System.Drawing.Size(75, 23);
            this.btnStatus.TabIndex = 2;
            this.btnStatus.Text = "Status";
            this.btnStatus.UseVisualStyleBackColor = true;
            this.btnStatus.Click += new System.EventHandler(this.btnStatus_Click);
            // 
            // mtbCNPJ
            // 
            this.mtbCNPJ.Location = new System.Drawing.Point(11, 56);
            this.mtbCNPJ.Mask = "99.999.999/9999-99";
            this.mtbCNPJ.Name = "mtbCNPJ";
            this.mtbCNPJ.Size = new System.Drawing.Size(126, 20);
            this.mtbCNPJ.TabIndex = 1;
            this.mtbCNPJ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mtbCNPJ.ValidatingType = typeof(System.DateTime);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "CNPJ";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(293, 159);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Geral";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pnlTestaFWSCRM
            // 
            this.pnlTestaFWSCRM.Controls.Add(this.btnSair);
            this.pnlTestaFWSCRM.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTestaFWSCRM.Location = new System.Drawing.Point(0, 184);
            this.pnlTestaFWSCRM.Name = "pnlTestaFWSCRM";
            this.pnlTestaFWSCRM.Size = new System.Drawing.Size(299, 40);
            this.pnlTestaFWSCRM.TabIndex = 1;
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.Location = new System.Drawing.Point(212, 9);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(75, 23);
            this.btnSair.TabIndex = 1;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // frmTeste
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 224);
            this.Controls.Add(this.pnlTestaFWSCRM);
            this.Controls.Add(this.tbcTestaFWSCRM);
            this.Name = "frmTeste";
            this.Text = "Testa FWSCRM";
            this.tbcTestaFWSCRM.ResumeLayout(false);
            this.tpeWebService.ResumeLayout(false);
            this.tpeWebService.PerformLayout();
            this.gbxSituacaoEmpresa.ResumeLayout(false);
            this.gbxSituacaoEmpresa.PerformLayout();
            this.pnlTestaFWSCRM.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcTestaFWSCRM;
        private System.Windows.Forms.TabPage tpeWebService;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel pnlTestaFWSCRM;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Button btnStatus;
        private System.Windows.Forms.MaskedTextBox mtbCNPJ;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbxAtivaDesativaEmpresa;
        private System.Windows.Forms.GroupBox gbxSituacaoEmpresa;
        private System.Windows.Forms.RadioButton rbnDesativada;
        private System.Windows.Forms.RadioButton rbnAtivada;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxUrlWebService;
    }
}

