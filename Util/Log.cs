using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FWSCRM.Util
{
    public sealed class LOG
    {
        #region Singleton

        //Variaveis Privadas
        private static volatile LOG i_instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static LOG Instance
        {
            get
            {
                if (i_instance == null)
                {
                    lock (typeof(LOG))
                    {
                        if (i_instance == null)
                        {
                            i_instance = new LOG();
                        }
                    }
                }
                return i_instance;
            }
        }

        //Inicialização Privada
        private LOG()
        {
        }

        #endregion

        #region Propriedades

        public String ArquivoLog { get; set; }

        private string i_CodigoEmpresa = null;
        public string CodigoEmpresa
        {
            get { return i_CodigoEmpresa; }
            set { i_CodigoEmpresa = value; }
        }

        private string i_LocalMonitoramento = null;
        public string LocalMonitoramento
        {
            get { return i_LocalMonitoramento; }
            set { i_LocalMonitoramento = value; }
        }

        private string i_DataOcorrencia = null;
        public string DataOcorrencia
        {
            get { return i_DataOcorrencia; }
            set { i_DataOcorrencia = value; }
        }

        private string i_HoraOcorrencia = null;
        public string HoraOcorrencia
        {
            get { return i_HoraOcorrencia; }
            set { i_HoraOcorrencia = value; }
        }

        private string i_Ocorrencia = null;
        public string Ocorrencia
        {
            get { return i_Ocorrencia; }
            set { i_Ocorrencia = value; }
        }

        private bool i_LogAtivo = false;
        public bool LogAtivo
        {
            get { return i_LogAtivo; }
            set { i_LogAtivo = value; }
        }

        private bool i_Incializando = false;
        public bool Incializando
        {
            get { return i_Incializando; }
            set { i_Incializando = value; }
        }

        #endregion

        #region Métodos

        public void RegistraLog()
        {
            if (LogAtivo)
            {

                this.DataOcorrencia = DateTime.Now.ToShortDateString();
                this.HoraOcorrencia = DateTime.Now.ToLongTimeString();

                try
                {
                    if (!File.Exists(ArquivoLog))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(ArquivoLog)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(ArquivoLog));
                        }
                    }

                    StreamWriter SW = new StreamWriter(ArquivoLog, true, Encoding.Default);
                    if (Incializando)
                    {
                        SW.WriteLine("");
                        SW.WriteLine("-------------------------------------------------------------------------------------------------------");
                        SW.WriteLine("Monitoramento da Empresa: " + CodigoEmpresa + " Incializando.");
                        SW.WriteLine("");
                    }

                    SW.WriteLine("Ponto da ocorrência: " + LocalMonitoramento);
                    SW.WriteLine("Data: " + DataOcorrencia + " - Hora: " + HoraOcorrencia);

                    if (!string.IsNullOrEmpty(Ocorrencia))
                    {
                        SW.WriteLine("Ocorrência: " + Ocorrencia);
                    }
                    SW.WriteLine("-------------------------------------------------------------------------------------------------------");
                    SW.WriteLine("");

                    if (Incializando)
                    {
                        SW.WriteLine("");
                        SW.WriteLine("Processamento da Empresa: " + CodigoEmpresa + " Inicializado.");
                        SW.WriteLine("-------------------------------------------------------------------------------------------------------");
                        SW.WriteLine("");

                    }

                    Incializando = false;
                    SW.Close();

                    //Limpar as propriedades.
                    this.DataOcorrencia = string.Empty;
                    this.HoraOcorrencia = string.Empty;
                    this.LocalMonitoramento = string.Empty;
                    this.Ocorrencia = string.Empty;
                }
                catch
                {
                    throw;
                }

            }
        }

        public void RegistraLog(Exception ex)
        {
            if (LogAtivo)
            {

                this.DataOcorrencia = DateTime.Now.ToShortDateString();
                this.HoraOcorrencia = DateTime.Now.ToLongTimeString();

                try
                {
                    if (!File.Exists(ArquivoLog))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(ArquivoLog)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(ArquivoLog));
                        }
                    }

                    StreamWriter SW = new StreamWriter(ArquivoLog, true, Encoding.Default);
                    if (Incializando)
                    {
                        SW.WriteLine("");
                        SW.WriteLine("-------------------------------------------------------------------------------------------------------");
                        SW.WriteLine("Monitoramento do Sistema - Inicialozando.");
                        SW.WriteLine("");
                    }

                    SW.WriteLine("Ponto da ocorrência: " + LocalMonitoramento);
                    SW.WriteLine("Data: " + DataOcorrencia + " - Hora: " + HoraOcorrencia);

                    if (!string.IsNullOrEmpty(Ocorrencia))
                    {
                        SW.WriteLine("Ocorrência: " + Ocorrencia);
                    }

                    
                    if (ex != null)
                    {
                        string l_Erro = ex.Message + Environment.NewLine;
                        if (ex.InnerException != null)
                        {
                            l_Erro += ex.InnerException.Message + Environment.NewLine; 
                        }
                        SW.WriteLine("Ocorrência: " + l_Erro);
                    }


                    SW.WriteLine("-------------------------------------------------------------------------------------------------------");
                    SW.WriteLine("");

                    Incializando = false;
                    SW.Close();

                    //Limpar as propriedades.
                    this.DataOcorrencia = string.Empty;
                    this.HoraOcorrencia = string.Empty;
                    this.LocalMonitoramento = string.Empty;
                    this.Ocorrencia = string.Empty;
                }
                catch
                {
                    throw;
                }

            }
        }
        #endregion
    }

}