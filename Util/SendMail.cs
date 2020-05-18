using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FWSCRM.Util
{
    public class SendMail
    {
        #region Variáveis

        private SmtpClient i_SmtpClient = null;

        private String PathLog { get; set; }

        public String EmailAssunto { get; set; }
        public String EmailDe { get; set; }
        public String EmailPara { get; set; }
        public String EmailCorpo { get; set; }

        public String EmailSMTP { get; set; }
        public int EmailPorta { get; set; }
        public String EmailUsuario { get; set; }
        public String EmailSenha { get; set; }
        
        public Attachment[] EmailAnexos { get; set; }
        public bool UsaSeguranca { get; set; }

        #endregion

        #region Propriedades

        private bool i_SENDMAIL_ATIVO = true;
        public bool SENDMAIL_ATIVO
        {
            get
            {
                return i_SENDMAIL_ATIVO;
            }
        }

        private string i_PM_PASTALOG = @"c:\NFe\Pastas\Log\";
        public string PM_PASTALOG
        {
            get
            {
                return i_PM_PASTALOG;
            }
            set
            {
                i_PM_PASTALOG = value;
            }
        }

        #endregion

        #region Inicialização

        public SendMail()
        {
            Inicializar();
        }

        public void Inicializar()
        {
            try
            {
                System.Reflection.Assembly l_Assembly = System.Reflection.Assembly.GetEntryAssembly();
                //PathLog = Path.GetDirectoryName(l_Assembly.Location);

                string l_Usuario, l_Senha, l_Porta, l_SMTP = string.Empty;

                bool l_UsaSeguranca = true;

                l_Usuario = EmailUsuario;
                l_Senha = EmailSenha;
                l_Porta = EmailPorta.ToString();
                l_SMTP = EmailSMTP;

                #region Endereço

                try
                {
                    i_SmtpClient = new SmtpClient(l_SMTP);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Erro ao configurar endereço do servidor SMTP", ex);
                }

                #endregion

                #region Credenciais

                NetworkCredential l_Credentials;

                try
                {
                    l_Credentials = new NetworkCredential(l_Usuario, l_Senha);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Erro ao configurar conta e senha do servidor SMTP", ex);
                }

                #endregion

                #region Domínio

                //try
                //{
                //    if (!String.IsNullOrEmpty(Global.Instance.Dominio))
                //    {
                //        l_Credentials.Domain = Global.Instance.Dominio;
                //    }
                //}
                //catch (Exception ex)
                //{
                //    throw new ApplicationException("Erro ao configurar domínio do servidor SMTP", ex);
                //}

                #endregion

                #region Segurança

                try
                {
                    i_SmtpClient.EnableSsl = Convert.ToBoolean(l_UsaSeguranca);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Erro ao configurar segurança do servidor SMTP", ex);
                }

                #endregion

                #region Porta

                try
                {

                    if (!String.IsNullOrEmpty(l_Porta))
                    {
                        i_SmtpClient.Port = Convert.ToInt32(l_Porta);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Erro ao configurar porta do servidor SMTP", ex);
                }

                #endregion

                #region Outras Propriedades SMTP

                i_SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                i_SmtpClient.UseDefaultCredentials = false;
                i_SmtpClient.Credentials = l_Credentials;
                i_SmtpClient.Timeout = 80000;

                #endregion

                i_SENDMAIL_ATIVO = true;
            }
            catch (ApplicationException ex)
            {
                i_SENDMAIL_ATIVO = false;
            }
            catch (Exception ex)
            {
                i_SENDMAIL_ATIVO = false;
            }
        }

        #endregion

        #region EnvioEmail
        public String Send()
        {
            Inicializar();

            string  l_Enviado = "OK";

            // Cria Thread pra disparar os emails
            ThreadStart threadStartSend = delegate { SendThread(); };
            Thread threadSend = new Thread(threadStartSend);

            try
            {
                threadSend.Start();
            }
            catch (Exception ex)
            {
                i_SENDMAIL_ATIVO = false;

                if (threadSend.IsAlive)
                {
                    threadSend.Abort();
                }

                l_Enviado = "Erro ao enviar email:" + ex.Message;
            }
            return l_Enviado;
        }

        private void SendThread()
        {
            try
            {
                MailMessage l_MailMessage = new MailMessage();

                Regex l_Regex = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

                l_MailMessage.Subject = EmailAssunto;
                l_MailMessage.Body = EmailCorpo;
                l_MailMessage.IsBodyHtml = true;
                
                //l_MailMessage.To.Add(new MailAddress(EmailPara));

                string[] l_EnderecosEmail = EmailPara.Split(',');

                foreach (string l_EnderecoEmail in l_EnderecosEmail)
                {
                    if (!l_Regex.IsMatch(l_EnderecoEmail))
                    {
                        l_MailMessage.From = new MailAddress(EmailUsuario);
                    }
                    else
                    {
                        l_MailMessage.From = new MailAddress(l_EnderecoEmail);
                    }
                    l_MailMessage.To.Add(l_EnderecoEmail);
                }

                foreach (Attachment l_Anexo in EmailAnexos)
                {
                    l_MailMessage.Attachments.Add(l_Anexo);
                }
                
                i_SmtpClient.Send(l_MailMessage);
            }
            catch (Exception ex)
            {
                GravarLog(PathLog, ex.Message.ToString());
            }
        }

        public bool Send(string a_Cidade, string a_UF, string a_Empresa, string a_CNPJ, string a_Email, string a_IE, string a_Data, string a_Telefone, string a_From, string a_Ambiente)
        {
            bool l_Enviado = false;

            // Cria Thread pra disparar os emails
            ThreadStart threadStartSend = delegate { SendThread(a_Cidade, a_UF, a_Empresa, a_CNPJ, a_Email, a_IE, a_Data, a_Telefone, a_From, a_Ambiente); };
            Thread threadSend = new Thread(threadStartSend);

            try
            {
                threadSend.Start();
                l_Enviado = true;
            }
            catch (Exception ex)
            {
                i_SENDMAIL_ATIVO = false;

                if (threadSend.IsAlive)
                {
                    threadSend.Abort();
                }

                l_Enviado = false;
            }
            return l_Enviado;
        }

        public bool SendNoThread(string a_Cidade, string a_UF, string a_Empresa, string a_CNPJ, string a_Banco, string a_Email, string a_IE, string a_Data, string a_Telefone, string a_From, string a_Ambiente)
        {
            bool l_Enviado = false;

            try
            {
                // Cria Thread pra disparar os emails
                SendThread(a_Cidade, a_UF, a_Empresa, a_CNPJ, a_Email, a_IE, a_Data, a_Telefone, a_From, a_Ambiente);

                l_Enviado = true;
            }
            catch (Exception ex)
            {
                i_SENDMAIL_ATIVO = false;

                l_Enviado = false;
                throw ex;
            }
            return l_Enviado;
        }

        private void SendThread(string a_Cidade, string a_UF, string a_Empresa, string a_CNPJ, string a_Email, string a_IE, string a_Data, string a_Telefone, string a_From, string a_Ambiente)
        {
            try
            {
                MailMessage l_MailMessage = new MailMessage();

                Regex l_Regex = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

                if (!l_Regex.IsMatch(a_From))
                {
                    l_MailMessage.From = new MailAddress("scrmconsultoria@gmail.com");
                }
                else
                {
                    l_MailMessage.From = new MailAddress(a_From);
                }
                l_MailMessage.To.Add(new MailAddress("scrmconsultoria@gmail.com"));

                l_MailMessage.Subject = EmailAssunto;
                l_MailMessage.Body = "EFiscal instalado na Empresa: " + a_Empresa + "\n CNPJ: " + a_CNPJ + "\n " + " IE: " + a_IE + " Cidade: " + a_Cidade + "\n UF: " + a_UF + " \n Email Cliente: " + a_Email + " Data: " + a_Data + " Telefone:" + a_Telefone + " Ambiente: " + a_Ambiente;
                l_MailMessage.IsBodyHtml = true;

                //l_MailMessage.Attachments.Add(new Attachment(a_XMLEnvio));

                i_SmtpClient.Send(l_MailMessage);
            }
            catch (Exception ex)
            {
                GravarLog(PathLog, ex.Message.ToString());
            }
        }

        private void GravarLog(string a_Path, string a_Mensagem)
        {
            StreamWriter l_SrLog = null;
            try
            {
                l_SrLog = new StreamWriter(a_Path + @"\LogInstall.txt", true, Encoding.UTF8);
                l_SrLog.WriteLine(DateTime.Now.ToString() + " - " + a_Mensagem);
                l_SrLog.Flush();
            }
            catch (Exception ex)
            {
                //abafa
            }
            finally
            {
                l_SrLog.Close();
            }
        }

        #endregion

        #region Template Email

        static public string ReadTemplateEmail()
        {
            string l_file = "";

            string l_path_arquivo = Application.StartupPath + @"\MonitorTemplateEmail.txt";

            if (File.Exists(l_path_arquivo))
            {
                StreamReader sr = new StreamReader(l_path_arquivo, Encoding.Default);
                l_file = sr.ReadToEnd();
                sr.Close();
            }

            return l_file;
        }

        static public string CreateEmailFromTemplate(ref StringDictionary templateParameters)
        {
            string email = ReadTemplateEmail();

            foreach (string key in templateParameters.Keys)
            {
                email = email.Replace(key, templateParameters[key]);
            }

            return email;
        }

        static public bool SaveTemplateEmail(string a_Template)
        {
            try
            {
                string l_path_arquivo = Application.StartupPath + @"\MonitorTemplateEmail.txt";
                File.WriteAllText(l_path_arquivo, a_Template, Encoding.Default);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
