using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using FWSCRM.Util;

namespace FWSCRM.XML
{
    public sealed class XML
    {
        #region Singleton

        //Variaveis Privadas
        private static volatile XML i_instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static XML Instance
        {
            get
            {
                if (i_instance == null)
                {
                    lock (typeof(XML))
                    {
                        if (i_instance == null)
                        {
                            i_instance = new XML();
                        }
                    }
                }
                return i_instance;
            }
        }

        //Inicialização Privada
        private XML()
        {
        }

        #endregion

        #region Propriedades

        public Byte[] XMLEnvio { get; set;}
        public Byte[] XMLRetorno { get; set; }

        public Byte[] XMLNFSe { get; set; }
        public Byte[] XMLCTe { get; set; }
        public Byte[] XMLRetornoCTe { get; set; }
        public Byte[] XMLDistribuicaoCTe { get; set; }
        public Byte[] XMLCancelamentoCTe { get; set; }
        public Byte[] XMLRetornoCancelamentoCTe { get; set; }
        public Byte[] XMLDistribuicaoCancelamentoCTe { get; set; }
        public Byte[] XMLInutilizacaoCTe { get; set; }
        public Byte[] XMLRetornoInutilizacaoCTe { get; set; }
        public Byte[] XMLDistribuicaoInutilizacaoCTe { get; set; }
        public Byte[] XMLContingenciaCTe { get; set; }
        public Byte[] XMLRetornoContingenciaCTe { get; set; }
        public Byte[] XMLDistribuicaoContingenciCTe { get; set; } 

        #endregion

        #region Métodos da Classe

        //public string CriarArqXMLStatusServico()
        //{
        //    string l_XmlStatus = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><consStatServ xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" versao=\" " + Global.Instance.i_VersaoStatusCTe + "\" xmlns=\"http://www.portalfiscal.inf.br/" + Global.Instance.SiglaDocEletronico + "\"><tpAmb>" + Global.Instance.CodigoAmbiente.ToString() + "</tpAmb><cUF>" + Global.Instance.UFCod.ToString() + "</cUF><xServ>STATUS</xServ></consStatServ>";

        //    string l_NomeArquivoStatus = Global.Instance.PastaEnvio + "\\" + DateTime.Now.ToString("yyyyMMddThhmmss") + "-ped-sta.xml";

        //    Util.Instance.GravarArquivoTexto(l_NomeArquivoStatus, l_XmlStatus);

        //    return l_NomeArquivoStatus;
        //}

        public void GravarXml(string a_Arquivo, string a_Xml)
        {
            XmlWriterSettings l_XmlWriterSettings = new XmlWriterSettings();
            l_XmlWriterSettings.Indent = true;
            l_XmlWriterSettings.IndentChars = "";
            l_XmlWriterSettings.NewLineOnAttributes = false;
            l_XmlWriterSettings.OmitXmlDeclaration = false;

            XmlWriter l_XmlWriter = XmlWriter.Create(a_Arquivo, l_XmlWriterSettings);
            XmlDocument l_xmlDocument = new XmlDocument();
            l_xmlDocument.LoadXml(a_Xml);
            l_xmlDocument.Save(l_XmlWriter);
            l_XmlWriter.Flush();
            l_XmlWriter.Close();
        }

        public string ObterNomeCompletoXmlPelaChave(string a_Path, string a_Chave)
        {

            #region Pesquisa na pasta Enviados

            DirectoryInfo l_DirectoryInfoEnviados = new DirectoryInfo(a_Path);
            FileInfo[] l_FileInfoEnviados;
            l_FileInfoEnviados = l_DirectoryInfoEnviados.GetFiles("*" + a_Chave + "*", SearchOption.AllDirectories);
            string l_XmlNotaEnviado = string.Empty;

            for (int i = 0; i <= l_FileInfoEnviados.Length - 1; i++)
            {
                #region Notas Série Única

                if (l_FileInfoEnviados[i].FullName.Contains("-nfe"))
                {
                    l_XmlNotaEnviado = l_FileInfoEnviados[i].FullName;
                }

                #endregion
            }

            #endregion

            return l_XmlNotaEnviado;
        }

        /// <summary>
        /// Converte Array de Bytes para Texto Unicode UTF-8.
        /// </summary>
        /// <param name="characters">Array de Bytes </param>
        /// <returns>String convertida do Array de Bytes</returns>
        public String CarregarXmlDocEletronico(Byte[] a_Bytes, string a_PastaEnvio)
        {
            UTF8Encoding l_UTF8Encoding = new UTF8Encoding();
            String l_StringXML = l_UTF8Encoding.GetString(a_Bytes);

            //Percorrer o xml e recuperar a chave.
            //Montar novamente o nome do arquivo.

            String[] l_ChaveArquivo = l_StringXML.Split('<');
            String[] l_IDArquivo = l_ChaveArquivo[3].Split('"');
            String l_NomeArquivo = l_IDArquivo[1].Replace('"', ' ');
            l_NomeArquivo = l_NomeArquivo + "-nfe.xml";
            String l_Path = Path.GetDirectoryName(a_PastaEnvio) + @"\";
            //Criar o arquivo na máquina.
            FileStream l_FileStream = new FileStream(l_Path + l_NomeArquivo, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            l_FileStream.Write(a_Bytes, 0, a_Bytes.Length);
            l_FileStream.Flush();
            l_FileStream.Close();

            //Retornar o nome gerado.
            return l_NomeArquivo;
        }

        /// <summary>
        /// Obtem MemoryStream de arquivo em Bytes
        /// </summary>
        /// <param name="a_Arquivo">Array de Bytes</param>
        /// <returns>MemoryStream</returns>
        //public MemoryStream ObterXmlMemoria(Byte[] a_Arquivo)
        //{
        //    MemoryStream l_MemoryStream = new MemoryStream(a_Arquivo);
        //    return l_MemoryStream;
        //}

        ///// <summary>
        ///// Transforma Arquivo XMl em Array de Bytes. Esse Arquvio esta em subpasta ( ano e mês de geração)
        ///// </summary>
        ///// <param name="a_Path">Pasta Armazenamento do arquivo XML </param>
        ///// <param name="a_Arquivo">Nome do Arquivo XML</param>
        ///// <returns>Array de bytes</returns>
        //private byte[] ObterXmlDocEletronico(string a_Path, string a_Arquivo)
        //{
        //    string l_Path = string.Empty;
        //    if (a_Path.Contains(":") & a_Arquivo.Contains(":"))
        //    {
        //        l_Path = a_Arquivo;
        //    }
        //    else if (a_Path.Contains(".xml"))
        //    {
        //        l_Path = a_Path;
        //    }
        //    else
        //    {
        //        l_Path = a_Path + @"\" + a_Arquivo;
        //    }

        //    if (!File.Exists(l_Path))
        //    {
        //        string l_AnoMes = DateTime.Now.ToString("yyyyMM") + @"\";
        //        l_Path = l_Path.Insert(l_Path.LastIndexOf("\\") + 1, l_AnoMes);
        //    }

        //    return Util.Instance.ObterArquivoBytes(l_Path);
        //}

        ///// <summary>
        ///// Ler arquivo XML e transforma em bytes
        ///// </summary>
        ///// <param name="a_Arquivo">Arquivo XML</param>
        ///// <returns>Array de bytes</returns>
        //private byte[] ObterXmlCancelamentoInutilizacaoContingencia(string a_Arquivo)
        //{
        //    return Util.Instance.ObterArquivoBytes(a_Arquivo);
        //}

        ///// <summary>
        ///// Ler arquivo XML de retorno da consulta e transforma em bytes
        ///// </summary>
        ///// <param name="a_Arquivo">Arquivo XML</param>
        ///// <returns>Array de bytes</returns>
        //private byte[] ObterXmlCancelamentoInutilizacaoContingenciaConsulta(string a_Arquivo, string a_Node)
        //{
        //    string l_NovoArquivo = a_Arquivo.Insert(a_Arquivo.LastIndexOf("\\") + 1, "tmp");

        //    StreamWriter l_StreamWriter = File.CreateText(l_NovoArquivo);
        //    l_StreamWriter.Write(Geral.Instance.ObterNode(a_Arquivo, a_Node));
        //    l_StreamWriter.Close();

        //    return Util.Instance.ObterArquivoBytes(l_NovoArquivo);
        //}

        #endregion

        #region Buscar Valor de TAG no XML

        /// <summary>
        /// Obtém o valor do elmento(Coluna) da Tag(Tabela) passada no Arquivo indicado na variável a_XML(DataSet)
        /// </summary>
        /// <param name="a_XML"> Nome do arquivo xml, inclusive com a path </param>
        /// <param name="a_Tag"> Tag onde será buscado o elemento (Significa a tabela dentro do DataSet) </param>
        /// <param name="a_Elemento"> Elemento que fornecerá o valor (Significa a coluna da tabela) </param>
        /// <returns> Valor do elemento </returns>
        public string ObterValorTagXML(Byte[] a_XML, String a_Tag, String a_Elemento)
        {
            try
            {
                string l_Valor = string.Empty;

                if (a_XML != null)
                {

                    DataSet l_DSXMLRetorno = new DataSet();
                    l_DSXMLRetorno.ReadXml(Geral.Instance.ObterStringMemoria(a_XML));
                    DataRow[] l_DataRow = l_DSXMLRetorno.Tables[a_Tag].Select();
                    l_Valor = l_DataRow[0][a_Elemento].ToString();
                }
                return l_Valor;
            }
            catch (Exception ex)
            {
                string l_Mensagem = string.Empty;
                if (ex.InnerException == null)
                {
                    l_Mensagem = ex.Message;
                }
                else
                {
                    l_Mensagem = ex.InnerException.Message;
                }

                throw new ApplicationException("Erro ao recuperar a Tag: (" + a_Tag + ") e elemento: (" + a_Elemento + ") no arquivo : (" + a_XML + ")" + " - " + l_Mensagem);
            }
        }

        public string ObterValorViaTag(byte[] a_XML, string a_Tag)
        {
            string l_ValorPesquisado = string.Empty;
            try
            {
                if (a_XML != null)
                {

                    DataSet l_DSXMLRetorno = new DataSet();
                    l_DSXMLRetorno.ReadXml(Geral.Instance.ObterStringMemoria(a_XML));
                    foreach (DataTable l_DataTable in l_DSXMLRetorno.Tables)
                    {
                        foreach (DataRow l_DataRow in l_DataTable.Rows)
                        {
                            for (int i = 0; i <= l_DataRow.Table.Columns.Count - 1; i++)
                            {
                                if (l_DataRow.Table.Columns[i].ColumnName.ToString() == a_Tag)
                                {
                                    l_ValorPesquisado = l_DataRow[l_DataRow.Table.Columns[i].ColumnName].ToString();
                                    return l_ValorPesquisado;
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return l_ValorPesquisado;
        }

        /// <summary>
        /// Verifica se existe Tag(Tabela) passada no Arquivo indicado na variável a_XML(DataSet)
        /// </summary>
        /// <param name="a_XML"> Nome do arquivo xml, inclusive com a path </param>
        /// <param name="a_Tag"> Tag onde será buscado o elemento (Significa a tabela dentro do DataSet) </param>
        /// <returns> verdadeiro ou falso </returns>
        public bool existeTableXML(Byte[] a_XML, String a_Tag)
        {
            try
            {

                bool l_ExisteTable = false;
                DataSet l_DSXMLRetorno = new DataSet();
                l_DSXMLRetorno.ReadXml(Geral.Instance.ObterStringMemoria(a_XML));
                foreach (DataTable l_DataTable in l_DSXMLRetorno.Tables)
                {
                    if (l_DataTable.TableName == a_Tag.Trim())
                    {
                        l_ExisteTable = true;
                        break;
                    }
                }
                return l_ExisteTable;
            }
            catch (Exception ex)
            {
                string l_Mensagem = string.Empty;
                if (ex.InnerException == null)
                {
                    l_Mensagem = ex.Message;
                }
                else
                {
                    l_Mensagem = ex.InnerException.Message;
                }

                throw new ApplicationException("Erro ao recuperar a tag: (" + a_Tag + ") no arquivo : (" + a_XML + ")" + " - " + l_Mensagem);
            }
        }

        /// <summary>
        /// Obtém o valor do elmento(Coluna) da Tag(Tabela) passada no Arquivo indicado na variável a_XML(DataSet)
        /// </summary>
        /// <param name="a_XML"> Nome do arquivo xml, inclusive com a path </param>
        /// <param name="a_Tag"> Tag onde será buscado o elemento (Significa a tabela dentro do DataSet) </param>
        /// <param name="a_Elemento"> Elemento que fornecerá o valor (Significa a coluna da tabela) </param>
        /// <returns> Valor do elemento </returns>
        //public string ObterValorTagXML(String a_XML, String a_Tag, String a_Elemento)
        //{
        //    try
        //    {
        //        //string l_AnoMes = DateTime.Now.ToString("yyyyMM") + @"\";
        //        string l_AnoMes = Global.Instance.DataEmissaoDocumentoFiscal.ToString("yyyyMM") + @"\";
        //        if (!File.Exists(a_XML))
        //        {
        //            a_XML = a_XML.Insert(a_XML.LastIndexOf("\\") + 1, l_AnoMes);
        //        }
        //        string l_Valor = string.Empty;
        //        DataSet l_DSXMLRetorno = new DataSet();

        //        l_DSXMLRetorno.ReadXml(a_XML);
        //        DataRow[] l_DataRow = l_DSXMLRetorno.Tables[a_Tag].Select();
        //        l_Valor = l_DataRow[0][a_Elemento].ToString();

        //        return l_Valor;
        //    }
        //    catch (Exception ex)
        //    {
        //        string l_Mensagem = string.Empty;
        //        if (ex.InnerException == null)
        //        {
        //            l_Mensagem = ex.Message;
        //        }
        //        else
        //        {
        //            l_Mensagem = ex.InnerException.Message;
        //        }

        //        throw new ApplicationException("Erro ao recuperar a tag: (" + a_Tag + ") e elemento: (" + a_Elemento + ") no arquivo : (" + a_XML + ")" + " - " + l_Mensagem);
        //    }
        //}


        /// <summary>
        /// Obtém o valor do elmento(Coluna) da Tag(grupo) fraguimento do xml passado
        /// </summary>
        /// <param name="a_XML"> Nome do fraguimento xml</param>
        /// <param name="a_Tag"> no da tag desejada </param>
        /// <returns> Valor do elemento </returns>
        public string ObterValorTagFraguimentoXML(String a_XML, String a_Tag)
        {
            try
            {
                string l_Valor = string.Empty;
                string[] l_Xml = a_XML.Split(new char[] { '<', '/' });
                foreach (String l_Elemento in l_Xml)
                {
                    if (l_Elemento.ToUpper().Contains(a_Tag.ToUpper()))
                    {
                        l_Valor = l_Elemento.Substring(l_Elemento.LastIndexOf(">") + 1);
                        break;
                    }
                }
                return l_Valor;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao recuperar a tag: (" + a_Tag + ")");
            }
        }

        /// <summary>
        /// Obtém o valor do elmento(Coluna) da Tag(Tabela) passada no Arquivo indicado na variável a_XML(DataSet)
        /// </summary>
        /// <param name="a_XML"> Nome do arquivo xml, inclusive com a path </param>
        /// <param name="a_Tag"> Tag onde será buscado o elemento (Significa a tabela dentro do DataSet) </param>
        /// <param name="a_Elemento"> Elemento que fornecerá o valor (Significa a coluna da tabela) </param>
        /// <returns> Valor do elemento </returns>
        public string ObterValorTagXML2(String a_XML, String a_Tag, String a_Elemento)
        {
            try
            {
                string l_Valor = string.Empty;

                XmlDocument l_XmlDocument = new XmlDocument();
                l_XmlDocument.Load(a_XML);

                XmlNodeList l_XmlNodeList = l_XmlDocument.GetElementsByTagName(a_Elemento);

                if (l_XmlNodeList.Count.Equals(0))
                {
                    l_XmlNodeList = l_XmlDocument.GetElementsByTagName("tc:" + a_Elemento);
                }

                if (l_XmlNodeList.Count.Equals(0))
                {
                    l_XmlNodeList = l_XmlDocument.GetElementsByTagName("ns2:" + a_Elemento);
                }

                if (l_XmlNodeList.Count.Equals(0))
                {
                    l_XmlNodeList = l_XmlDocument.GetElementsByTagName("ns3:" + a_Elemento);
                }

                if (l_XmlNodeList.Count.Equals(0))
                {
                    l_XmlNodeList = l_XmlDocument.GetElementsByTagName("ns4:" + a_Elemento);
                }

                if (l_XmlNodeList.Count.Equals(0))
                {
                    throw new Exception();
                }

                foreach (var l_XMLNode in l_XmlNodeList)
                {
                    XmlNode l_ParentNode = ((XmlNode)l_XMLNode).ParentNode;
                    string l_ParentNodeName = l_ParentNode.Name;

                    if (l_ParentNodeName.Equals(a_Tag) || l_ParentNodeName.Equals("tc:" + a_Tag)
                        || l_ParentNodeName.Equals("ns2:" + a_Tag) || l_ParentNodeName.Equals("ns3:" + a_Tag)
                        || l_ParentNodeName.Equals("ns4:" + a_Tag))
                    {
                        l_Valor = ((XmlNode)l_XMLNode).LastChild.Value;
                        return l_Valor;
                    }
                }

                return l_Valor;
            }
            catch (Exception)
            {
                throw new ApplicationException("Erro ao recuperar a tag: (" + a_Tag + ") e elemento: (" + a_Elemento + ") no arquivo : (" + a_XML + ")");
            }
        }

        /// <summary>
        /// Obtém os valores do elmento(Coluna) da Tag(Tabela) passada no Arquivo indicado na variável a_XML(DataSet)
        /// </summary>
        /// <param name="a_XML"> Nome do arquivo xml, inclusive com a path </param>
        /// <param name="a_Tag"> Tag onde será buscado o elemento (Significa a tabela dentro do DataSet) </param>
        /// <param name="a_Elemento"> Elemento que fornecerá o valor (Significa a coluna da tabela) </param>
        /// <returns> Valor do elemento </returns>
        public string ObterValorTagXMLLista(String a_XML, String a_Tag, String a_Elemento)
        {
            try
            {
                string l_Valor = string.Empty;

                XmlDocument l_XmlDocument = new XmlDocument();
                //l_XmlDocument.Load(a_XML);
                l_XmlDocument.LoadXml(a_XML);

                XmlNodeList l_XmlNodeList = l_XmlDocument.GetElementsByTagName(a_Elemento);

                if (l_XmlNodeList.Count.Equals(0))
                {
                    throw new Exception();
                }

                foreach (var l_XMLNode in l_XmlNodeList)
                {
                    XmlNode l_ParentNode = ((XmlNode)l_XMLNode).ParentNode;
                    string l_ParentNodeName = l_ParentNode.Name;

                    if (l_ParentNodeName.Equals(a_Tag))
                    {
                        l_Valor = l_Valor + ((XmlNode)l_XMLNode).LastChild.Value;
                    }
                }

                return l_Valor.Trim();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Elemento raiz inexistente."))
                {
                    throw new ApplicationException("O WebService não retornou arquivo.");
                }
                else
                {
                    throw new ApplicationException("Erro ao recuperar a tag: (" + a_Tag + ") e elemento: (" + a_Elemento + ") no arquivo : (" + a_XML + ")");
                }
            }
        }

        /// <summary>
        /// Verifica se existe Tag(Tabela) passada no Arquivo indicado na variável a_XML(DataSet)
        /// </summary>
        /// <param name="a_XML"> Nome do arquivo xml, inclusive com a path </param>
        /// <param name="a_Tag"> Tag onde será buscado o elemento (Significa a tabela dentro do DataSet) </param>
        /// <returns> verdadeiro ou falso </returns>
        public bool existeTableXML(String a_XML, String a_Tag)
        {
            try
            {
                string l_AnoMes = DateTime.Now.ToString("yyyyMM") + @"\";
                if (!File.Exists(a_XML))
                {
                    a_XML = a_XML.Insert(a_XML.LastIndexOf("\\") + 1, l_AnoMes);
                }

                bool l_ExisteTable = false;
                DataSet l_DSXMLRetorno = new DataSet();

                l_DSXMLRetorno.ReadXml(a_XML);
                foreach (DataTable l_DataTable in l_DSXMLRetorno.Tables)
                {
                    if (l_DataTable.TableName == a_Tag.Trim())
                    {
                        l_ExisteTable = true;
                        break;
                    }
                }
                return l_ExisteTable;
            }
            catch (Exception ex)
            {
                string l_Mensagem = string.Empty;
                if (ex.InnerException == null)
                {
                    l_Mensagem = ex.Message;
                }
                else
                {
                    l_Mensagem = ex.InnerException.Message;
                }

                throw new ApplicationException("Erro ao recuperar a tag: (" + a_Tag + ") no arquivo : (" + a_XML + ")" + " - " + l_Mensagem);
            }
        }

        #endregion
    }
}
