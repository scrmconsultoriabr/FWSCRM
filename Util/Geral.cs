using Ionic.Zip;
using iTextSharp.text.pdf;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FWSCRM.Util
{
    public sealed class Geral
    {
        #region Singleton

        //Variaveis Privadas
        private static volatile Geral i_instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static Geral Instance
        {
            get
            {
                if (i_instance == null)
                {
                    lock (typeof(Geral))
                    {
                        if (i_instance == null)
                        {
                            i_instance = new Geral();
                        }
                    }
                }
                return i_instance;
            }
        }

        //Inicialização Privada
        private Geral()
        {
        }

        #endregion

        #region Propriedades

        //PortalProjetos.PortalProjetosSoap lPortalProjetos; // = new PortalProjetos.PortalProjetosSoap();

        //var correios = new Correios.AtendeClienteClient();
        //var consulta = correios.consultaCEPAsync(cep.Codigo.Replace("-", "")).Result;


        PortalProjetos.PortalProjetosSoapClient i_PortalProjetos = new PortalProjetos.PortalProjetosSoapClient(PortalProjetos.PortalProjetosSoapClient.EndpointConfiguration.PortalProjetosSoap12);
        

        //LPortalProjetos   i_PortalProjetos = new PortalProjetos.EmpresaAutenticadaRequest();


        #endregion

        #region Métodos Gerais

        /*
         Console.WriteLine(Indent(0) + "List");
         Console.WriteLine(Indent(3) + "Item 1");
         Console.WriteLine(Indent(6) + "Item 1.1");
         Console.WriteLine(Indent(6) + "Item 1.2");
         Console.WriteLine(Indent(3) + "Item 2");
         Console.WriteLine(Indent(6) + "Item 2.1");

         Output string:

         List
            Item 1
               Item 1.1
               Item 1.2
            Item 2
               Item 2.1
         */
        public static string Indent(int count)
        {
            return "".PadLeft(count);
            
        }

        public DateTime DataServidor()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Recebe arquivo em bytes e devolve em string
        /// </summary>
        /// <param name="a_Arquivo">Arquivo em Bytes</param>
        /// <returns>String</returns>
        public String ObterStringBytes(Byte[] a_Arquivo)
        {
            UTF8Encoding l_UTF8Encoding = new UTF8Encoding();
            return l_UTF8Encoding.GetString(a_Arquivo);
        }

        public String PegarArquivo(string a_Path, string a_Arquivo, string a_Chave)
        {
            DirectoryInfo l_DirectoryInfo = new DirectoryInfo(a_Path);
            FileInfo[] l_FileInfo;
            l_FileInfo = l_DirectoryInfo.GetFiles("*" + a_Arquivo + "*", SearchOption.AllDirectories);

            string l_Arquvio = string.Empty;

            for (int i = 0; i <= l_FileInfo.Length - 1; i++)
            {
                #region Notas Série Única

                if (l_FileInfo[i].FullName.Contains(a_Arquivo + a_Chave))
                {
                    l_Arquvio = l_FileInfo[i].FullName;
                }

                #endregion
            }
            return l_Arquvio;
        }

        public String ObterHash(string a_Numero)
        {
            // Funcionando para presitente vesceslau (Igor)
            byte[] data = new byte[a_Numero.Length];


            SHA1 sha = new System.Security.Cryptography.SHA1Managed();
            sha.ComputeHash(System.Text.Encoding.Default.GetBytes(a_Numero));

            return Convert.ToBase64String(sha.Hash);
        }

        /// <summary>
        /// Recebe um cadeia de caracter e filtra retornando apenas números
        /// </summary>
        /// <param name="a_Numero">Cadeia de caracter</param>
        /// <returns>Numeros contidos na cadeia de caracter passado</returns>
        public string ObterNumerosString(string a_Numero)
        {
            string l_Retorno = string.Empty;
            foreach (char l_Digito in a_Numero)
            {
                if (Char.IsDigit(l_Digito))
                {
                    l_Retorno += l_Digito;

                }
            }
            return l_Retorno;
        }

        /// <summary>
        /// Recebe um cadeia de caracter e filtra retornando apenas letras
        /// </summary>
        /// <param name="a_Numero">Cadeia de caracter</param>
        /// <returns>Letras contidos na cadeia de caracter passado</returns>
        public string ObterStringNumeros(string a_Numero)
        {
            string l_Retorno = string.Empty;
            foreach (char l_Digito in a_Numero)
            {
                if (Char.IsLetter(l_Digito))
                {
                    l_Retorno += l_Digito;
                }
            }
            return l_Retorno;
        }

        /// <summary>
        /// Valida CNPJ
        /// </summary>
        /// <param name="a_CNPJ">CNPJ a ser validado</param>
        /// <returns>Verdadeio ou Falso</returns>
        public bool ValidarCnpj(String a_CNPJ)
        {
            Int32[] l_Digitos, l_Soma, l_Resultado;

            Int32 l_NumeroDigitos;

            String l_FormulaMT;

            Boolean[] l_CNPJOK;

            a_CNPJ = a_CNPJ.Replace("/", "");

            a_CNPJ = a_CNPJ.Replace(".", "");

            a_CNPJ = a_CNPJ.Replace("-", "");

            if (a_CNPJ == "00000000000000")
            {
                return false;
            }

            l_FormulaMT = "6543298765432";

            l_Digitos = new Int32[14];

            l_Soma = new Int32[2];

            l_Soma[0] = 0;

            l_Soma[1] = 0;

            l_Resultado = new Int32[2];

            l_Resultado[0] = 0;

            l_Resultado[1] = 0;

            l_CNPJOK = new Boolean[2];

            l_CNPJOK[0] = false;

            l_CNPJOK[1] = false;

            try
            {

                for (l_NumeroDigitos = 0; l_NumeroDigitos < 14; l_NumeroDigitos++)
                {

                    l_Digitos[l_NumeroDigitos] = int.Parse(a_CNPJ.Substring(l_NumeroDigitos, 1));

                    if (l_NumeroDigitos <= 11)

                        l_Soma[0] += (l_Digitos[l_NumeroDigitos] *

                        int.Parse(l_FormulaMT.Substring(l_NumeroDigitos + 1, 1)));

                    if (l_NumeroDigitos <= 12)

                        l_Soma[1] += (l_Digitos[l_NumeroDigitos] *

                        int.Parse(l_FormulaMT.Substring(l_NumeroDigitos, 1)));

                }

                for (l_NumeroDigitos = 0; l_NumeroDigitos < 2; l_NumeroDigitos++)
                {

                    l_Resultado[l_NumeroDigitos] = (l_Soma[l_NumeroDigitos] % 11);

                    if ((l_Resultado[l_NumeroDigitos] == 0) || (l_Resultado[l_NumeroDigitos] == 1))

                        l_CNPJOK[l_NumeroDigitos] = (l_Digitos[12 + l_NumeroDigitos] == 0);

                    else

                        l_CNPJOK[l_NumeroDigitos] = (l_Digitos[12 + l_NumeroDigitos] == (

                        11 - l_Resultado[l_NumeroDigitos]));

                }

                return (l_CNPJOK[0] && l_CNPJOK[1]);

            }

            catch
            {

                return false;

            }

        }

        /// <summary>
        /// Valida CPF
        /// </summary>
        /// <param name="a_CPF">CPF a ser validado</param>
        /// <returns>Verdadeiro ou Falso</returns>
        public bool ValidarCPF(string a_CPF)
        {
            int[] l_Numeros = new int[11];
            string l_Valor = a_CPF.Replace(".", "");
            bool l_Igual = true;
            int l_Soma = 0;

            l_Valor = l_Valor.Replace("-", "");

            if (l_Valor.Length != 11)
            {
                return false;
            }

            for (int i = 1; i < 11 && l_Igual; i++)
            {
                if (l_Valor[i] != l_Valor[0])
                {
                    l_Igual = false;
                }
            }

            if (l_Igual || l_Valor == "12345678909")
            {
                return false;
            }

            for (int i = 0; i < 11; i++)
            {
                l_Numeros[i] = int.Parse(l_Valor[i].ToString());
            }

            for (int i = 0; i < 9; i++)
            {
                l_Soma += (10 - i) * l_Numeros[i];
            }

            int l_Resultado = l_Soma % 11;

            if (l_Resultado == 1 || l_Resultado == 0)
            {
                if (l_Numeros[9] != 0)
                {
                    return false;
                }
            }
            else if (l_Numeros[9] != 11 - l_Resultado)
            {
                return false;
            }

            l_Soma = 0;

            for (int i = 0; i < 10; i++)
            {
                l_Soma += (11 - i) * l_Numeros[i];
            }

            l_Resultado = l_Soma % 11;

            if (l_Resultado == 1 || l_Resultado == 0)
            {
                if (l_Numeros[10] != 0)
                {
                    return false;
                }
            }
            else if (l_Numeros[10] != 11 - l_Resultado)
            {
                return false;
            }
            return true;
        }

        #region Tratar Caracteres

        public string RetirarAcentos(string texto)
        {
            if (texto != null)
            {
                string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
                string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";
                for (int i = 0; i < comAcentos.Length; i++)
                {
                    texto = texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
                }
            }

            return texto;
        }

        public string LimparTexto(string a_Texto)
        {
            string l_Texto = string.Empty;
            Boolean l_TemCaracterInvalido = false;
            if (a_Texto != null)
            {
                string l_CaracterInvalido = @"',./-\(<>);:¨¬¢£çÇ|ªº₢_=+&$%#@!?*´´~^{}[]";
                for (int i = 0; i < a_Texto.Length; i++)
                {
                    l_TemCaracterInvalido = false;
                    for (int j = 0; j < l_CaracterInvalido.Length; j++)
                    {
                        if (a_Texto[i] == l_CaracterInvalido[j])
                        {
                            l_TemCaracterInvalido = true;
                            break;
                        }
                    }
                    if (!l_TemCaracterInvalido)
                    {
                        l_Texto += a_Texto[i];
                    }

                }
            }

            return l_Texto;
        }

        #endregion

        /// <summary>
        /// Seta TabPage para visível ou invisível
        /// </summary>
        /// <param name="a_TabControl">TabControl que contem a TabPage</param>
        /// <param name="a_TabPage">TabPage</param>
        /// <param name="a_Visible">Opção</param>
        public void SetTabPageVisible(TabControl a_TabControl, TabPage a_TabPage, bool a_Visible)
        {
            a_TabControl.TabPages.Remove(a_TabPage);
            if (a_Visible)
            {
                a_TabControl.TabPages.Add(a_TabPage);
            }
        }

        public string MimeType(string a_Extensao)
        {
            string l_Mime = "application/octetstream";
            if (string.IsNullOrEmpty(a_Extensao))
            {
                return l_Mime;
            }

            string l_ext = a_Extensao.ToLower();
            Microsoft.Win32.RegistryKey l_RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(l_ext);
            if (l_RegistryKey != null && l_RegistryKey.GetValue("Content Type") != null)
            {
                l_Mime = l_RegistryKey.GetValue("Content Type").ToString();
            }

            return l_Mime;
        }

        public string GerarModulo11(string a_Numero)
        {
            int[] l_Pesos = { 2, 3, 4, 5, 6, 7, 8, 9 };
            string l_Text = a_Numero.ToString();

            int l_Soma = 0;
            int l_Idx = 0;

            for (int l_Pos = l_Text.Length - 1; l_Pos >= 0; l_Pos--)
            {
                l_Soma += Convert.ToInt32(l_Text[l_Pos].ToString()) * l_Pesos[l_Idx];
                l_Idx++;
                if (l_Idx == 8)
                    l_Idx = 0;
            }

            //int a_Resto = (a_Soma * 10) % 11;
            int l_Resto = l_Soma % 11;
            int l_Digito = 11 - l_Resto;
            if (l_Digito >= 10)
                l_Digito = 0;

            return l_Digito.ToString();
        }


        public string StrZero(int a_Num, int a_Zeros, int a_Deci)
        {
            int l_Tam;
            string l_Retorno, l_Res, l_Zer;

            l_Res = a_Num.ToString() + a_Zeros.ToString() + a_Deci.ToString();
            l_Tam = l_Res.Length;
            l_Zer = "";
            for (int l_z = 1; l_z <= (a_Zeros - l_Tam); l_z++)
                l_Zer = l_Zer + '0';

            l_Retorno = l_Zer + l_Res;
            return l_Retorno;
        }

        public string StrZeroL(int a_Num, int a_QtdZeros)
        {
            string l_Retorno = a_Num.ToString();
            for (int l_i = 1; l_i <= a_QtdZeros; l_i++)
            {
                if (l_Retorno.Length < a_QtdZeros)
                    l_Retorno = '0' + l_Retorno;

            }
            return l_Retorno;
        }

        public string StrZeroL(string a_Num, int a_QtdZeros)
        {
            string l_Retorno = a_Num;
            for (int l_i = 1; l_i <= a_QtdZeros; l_i++)
            {
                if (l_Retorno.Length < a_QtdZeros)
                    l_Retorno = '0' + l_Retorno;

            }
            return l_Retorno;
        }

        public string StrZeroR(int a_Num, int a_QtdZeros)
        {
            string l_Retorno = a_Num.ToString();
            for (int l_i = 1; l_i <= a_QtdZeros; l_i++)
            {
                if (l_Retorno.Length < a_QtdZeros)
                    l_Retorno = '0' + l_Retorno;

            }
            return l_Retorno;
        }
        #endregion

        #region Métodos tratamento de arquivos

        /// <summary>
        /// Gera arquivo com o conteudo passado
        /// </summary>
        /// <param name="a_Arquivo">Nome do arquivo </param>
        /// <param name="a_Conteudo">Conteúdo do arquivo</param>
        public void GravarArquivoTexto(string a_Arquivo, string a_Conteudo)
        {
            StreamWriter l_StreamWriter = File.CreateText(a_Arquivo);
            l_StreamWriter.Write(a_Conteudo);
            l_StreamWriter.Close();
        }

        /// <summary>
        /// Gera arquivo com o conteudo passado
        /// </summary>
        /// <param name="a_Arquivo">Nome do arquivo </param>
        /// <param name="a_Conteudo">Conteúdo do arquivo</param>
        /// <param name="a_CriaPath">Cria a doretorio se não existir</param>
        public void GravarArquivoTexto(string a_Arquivo, string a_Conteudo, bool a_CriaPath)
        {
            if (a_CriaPath)
            {
                if (!Directory.Exists(Path.GetDirectoryName(a_Arquivo)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(a_Arquivo));
                }
            }
            StreamWriter l_StreamWriter = File.CreateText(a_Arquivo);
            l_StreamWriter.Write(a_Conteudo);
            l_StreamWriter.Close();
        }

        /// <summary>
        /// Ler texto de arquivo passado
        /// </summary>
        /// <param name="a_NomeArquivo">Arquivo</param>
        /// <returns>Retorna string do arquivo passado</returns>
        public string ObterArquivoTexto(string a_NomeArquivo)
        {
            StreamReader l_StreamReader;
            l_StreamReader = File.OpenText(a_NomeArquivo);
            string l_Texto = l_StreamReader.ReadToEnd();
            l_StreamReader.Close();

            return l_Texto;
        }

        /// <summary>
        /// Transforma arquivo em array de bytes
        /// </summary>
        /// <param name="a_Arquivo">Nome completo do arquivo</param>
        /// <returns>Array de bytes</returns>
        public byte[] ObterArquivoBytes(string a_Arquivo)
        {
            FileStream l_FileStream = File.Open(a_Arquivo, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            byte[] byteArray = new byte[l_FileStream.Length];
            l_FileStream.Read(byteArray, 0, (int)l_FileStream.Length);
            l_FileStream.Close();
            return byteArray;
        }

        /// <summary>
        /// Obtem MemoryStream de arquivo em Bytes
        /// </summary>
        /// <param name="a_Arquivo">Array de Bytes</param>
        /// <returns>MemoryStream</returns>
        public MemoryStream ObterStringMemoria(Byte[] a_Arquivo)
        {
            MemoryStream l_MemoryStream = new MemoryStream(a_Arquivo);
            return l_MemoryStream;
        }

        /// <summary>
        /// Obtem MemoryStream de String
        /// </summary>
        /// <param name="a_Arquivo">String</param>
        /// <returns>MemoryStream</returns>
        public MemoryStream ObterStringMemoria(String a_ArquivoTexto)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            MemoryStream l_MemoryStream = new MemoryStream(encoding.GetBytes(a_ArquivoTexto));
            return l_MemoryStream;
        }

        /// <summary>
        /// Obtem MemoryStream de arquivo em Bytes
        /// </summary>
        /// <param name="a_Arquivo">Array de Bytes</param>
        /// <returns>MemoryStream</returns>
        public Byte[] ObterBytesString(String a_ArquivoTexto)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(a_ArquivoTexto);
        }

        public Image ObterImagemLogo(string a_Path)
        {
            System.Drawing.Image l_Image = null;
            try
            {
                byte[] l_ImagemDisco = Geral.Instance.ObterArquivoBytes(a_Path);
                MemoryStream l_MemoryStream = new MemoryStream(l_ImagemDisco);
                l_Image = System.Drawing.Image.FromStream(l_MemoryStream);
            }
            catch 
            {
                return l_Image;
            }
            return l_Image;
        }

       
        /// <summary>
        /// Recebe arquivo em bytes e devolce Arquivo em formato texto
        /// </summary>
        /// <param name="a_Arquivo">Arquivo Bytes</param>
        /// <returns>Arquivo Texto</returns>
        public String ObterArquivoTexto(Byte[] a_Arquivo)
        {
            UTF8Encoding l_UTF8Encoding = new UTF8Encoding();
            return l_UTF8Encoding.GetString(a_Arquivo);
        }

        /// <summary>
        /// Gera codigo de barra de devolve imagem
        /// </summary>
        /// <param name="a_Chave">Chave</param>
        /// <returns>Imagem da Chave em código de barras</returns>
        private byte[] GerarCodigoBarras(string a_Chave)
        {

            BarcodeInter25 inter = new BarcodeInter25();

            Barcode128 bar = new Barcode128();

            bar.CodeType = Barcode128.CODE_C;

            bar.Code = a_Chave;

            bar.BarHeight = 50;

            bar.N = 3;

            System.Drawing.Image img = bar.CreateDrawingImage(Color.Black, Color.White);

            using (MemoryStream ms = new MemoryStream())
            {

                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                return ms.ToArray();

            }

        }

        public String GerarCodigoBarras(string a_Arquivo, string a_Chave)
        {
            Byte[] l_CodigoBarras = GerarCodigoBarras(a_Chave);
            FileStream l_FileStream = null;
            // create a write stream
            try
            {
                l_FileStream = new FileStream(a_Arquivo, FileMode.Create, FileAccess.Write);
                l_FileStream.Write(l_CodigoBarras, 0, l_CodigoBarras.Length);
                //fs.Write(buffer, 0, bytesRead);

                //ReadWriteStream(l_FileStream, l_FileStream);
            }
            finally
            {
                l_FileStream.Close();
            }
            return a_Arquivo;

        }

        // readStream is the stream you need to read
        // writeStream is the stream you want to write to
        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

        /// <summary>
        /// Rebe enum e devol a description
        /// </summary>
        /// <param name="a_Item">Enum</param>
        /// <returns></returns>
        public String DescricaoEnum(Enum a_Item)
        {
            var l_Tipo = a_Item.GetType();
            FieldInfo l_FieldInfo = l_Tipo.GetField(a_Item.ToString());

            var l_Atributos = l_FieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (l_Atributos.Length > 0)
                return l_Atributos[0].Description;
            else
                return String.Empty;
        }

        #endregion

        #region Serialização de Objetos

        /// <summary>
        /// Serializa um objeto (Classe) em XML
        /// Exêmplo: string xml = Serializar(cliente); 
        /// Obs.: Cliente é uma classe como valores
        /// </summary>
        /// <param name="algumObjeto">Objeto Classe</param>
        /// <returns>Objeto serializado</returns>
        public string Serializar(object algumObjeto)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new
            XmlSerializer(algumObjeto.GetType());
            serializer.Serialize(writer, algumObjeto);
            return writer.ToString();
        }

        /// <summary>
        /// Deserializa objeto de acordo com seu conteúdo e tipo
        /// Exêmplo: Cliente clienteDeserializado = Deserializar(xml, typeof(Cliente)) as Cliente;
        /// </summary>
        /// <param name="xml">conteúdo</param>
        /// <param name="type">tipo</param>
        /// <returns>Objeto deserializado</returns>
        public object Deserializar(string xml, Type type)
        {
            StringReader reader = new StringReader(xml);
            XmlSerializer serializer = new XmlSerializer(type);
            return serializer.Deserialize(reader);
        }

        #endregion

        #region Métodos para compactação de arquivos

        public void CompactarDiretorio(String a_Diretorio, String a_NomeZip)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.StatusMessageTextWriter = System.Console.Out;
                    zip.AddDirectory(a_Diretorio); // recurses subdirectories
                    zip.Save(a_NomeZip);
                }
            }
            catch (Exception ex)
            {
                new Exception(ex.Message);
            }
            finally
            { 

            }
        }

        public void CompactarArquivos(String a_Diretorio, String a_NomeZip)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    String[] l_ArquviosPath = System.IO.Directory.GetFiles(a_Diretorio);

                    // This is just a sample, provided to illustrate the DotNetZip interface.  
                    // This logic does not recurse through sub-directories.
                    // If you are zipping up a directory, you may want to see the AddDirectory() method, 
                    // which operates recursively. 
                    foreach (String filename in l_ArquviosPath)
                    {
                        ZipEntry e = zip.AddFile(filename);
                        //e.Comment = "Added by Cheeso's CreateZip utility.";
                    }

                    //zip.Comment = String.Format("This zip archive was created by the CreateZip example application on machine '{0}'",  System.Net.Dns.GetHostName());

                    zip.Save(a_NomeZip);
                }
            }
            catch (Exception ex)
            {
                new Exception(ex.Message);
            }
            finally
            {

            }
        }

        public void ExtrairArquivoZipado(String a_NomeZip)
        {
            try
            {
                // Specifying Console.Out here causes diagnostic msgs to be sent to the Console
                // In a WinForms or WPF or Web app, you could specify nothing, or an alternate
                // TextWriter to capture diagnostic messages.

                var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
                using (ZipFile zip = ZipFile.Read(a_NomeZip, options))
                {
                    // This call to ExtractAll() assumes:
                    //   - none of the entries are password-protected.
                    //   - want to extract all entries to current working directory
                    //   - none of the files in the zip already exist in the directory;
                    //     if they do, the method will throw.
                    zip.ExtractAll(a_NomeZip);
                }
            }
            catch (Exception ex)
            {
                new Exception(ex.Message);
            }
        }

        #endregion

        #region Métodos Autenticação SE

        public bool EFiscalPermissaoUso(DataTable l_DataSetEmpresa, String a_URL)
        {
            //Seta Variável para verificar autenticação da empresa
            Boolean l_EmpresaAutenticada = false;
            
            //Guarda URL para trabalhar em ambiente de desenvolvimento
            string l_UrlAnterior = i_PortalProjetos.Endpoint.Address.Uri.AbsoluteUri; // .Url;

            foreach (DataRow l_Empresa in l_DataSetEmpresa.Rows)
            {
                try
                {
                    try
                    {
                        //Tenta conexão com url informada
                        l_EmpresaAutenticada = VerificaSituacaoEmpresa(l_Empresa["emp_nr_cnpj"].ToString(), a_URL, 5000);
                        //l_EmpresaAutenticada = i_PortalProjetos.EmpresaAutenticadaAsync(l_Empresa["emp_nr_cnpj"].ToString()).Result.Body.EmpresaAutenticadaResult;
                    }
                    catch
                    {
                        //Usar a url de desenvolvimento
                        //i_PortalProjetos.Url = l_UrlAnterior;
                        //Configura Serviço
                        
                        l_EmpresaAutenticada = VerificaSituacaoEmpresa(l_Empresa["emp_nr_cnpj"].ToString(), l_UrlAnterior, 5000);
                        //l_EmpresaAutenticada = i_PortalProjetos.EmpresaAutenticadaAsync(l_Empresa["emp_nr_cnpj"].ToString()).Result.Body.EmpresaAutenticadaResult;
                    }

                    //Basta apenas uma empresa do grupo não esta autenticada para que o sistema interrompa a execução
                    if (!l_EmpresaAutenticada)
                    {
                        break;
                    }
                }
                //Aqui quando o sistema "PortalProjetos estiver fora do ar"
                catch 
                {
                    l_EmpresaAutenticada = true;
                }
            }
            return l_EmpresaAutenticada;
        }

        /// <summary>
        /// Obtem Situação da Empresa
        /// </summary>
        /// <param name="PCNPJ">CNPJ a ser consultado</param>
        /// <param name="PUrl">URL WebService</param>
        /// <param name="PTimeOut">Tempo de espera</param>
        /// <returns></returns>
        public bool VerificaSituacaoEmpresa(string PCNPJ, string PUrl, int PTimeOut)
        {
            //Configura Serviço
            ConfiguraServico(PUrl, PTimeOut);

            //Obtem Retorno WebService
            PortalProjetos.EmpresaAutenticadaResponse lResponse = i_PortalProjetos.EmpresaAutenticadaAsync(PCNPJ).Result;

            //Obtem Situação da Empresa
            return lResponse.Body.EmpresaAutenticadaResult;
        }

        private void ConfiguraServico(string PUrl, int PTimeOut)
        {
            // i_PortalProjetos.Url = a_URL;
            i_PortalProjetos.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(PUrl));
            //i_PortalProjetos.Timeout = 5000;
            System.ServiceModel.BasicHttpBinding lBinding = new System.ServiceModel.BasicHttpBinding();
            lBinding.ReceiveTimeout.Add(System.TimeSpan.FromSeconds(PTimeOut));
            i_PortalProjetos.Endpoint.Binding = lBinding;
        }

        #endregion

        public void RefreshMemoria()
        {
            MemoryManagement l_MemoryManagement = new MemoryManagement();
            l_MemoryManagement.FlushMemory();
        }
       
    }

   
    #region Limpa Memória

    public class MemoryManagement
    {
        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        public void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
    }

    #endregion

}
