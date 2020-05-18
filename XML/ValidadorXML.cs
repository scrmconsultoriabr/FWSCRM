using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using FWSCRM.Util;

namespace FWSCRM.XML
{
    /// <summary>
    /// 1)Modificado o método ValidarXML, foram comentadas as linhas de descrição do erro: 
    /// this.RetornoString = "Início da validação...\r\n\r\n";
    /// this.RetornoString += "\r\n...Final da validação";  
    /// 2)Modificado o método ValidarXML, o retorno do método foi alterado de void para string e a linha a seguir foi incluída no fim do método.
    /// return this.RetornoString;
    /// </summary>
    public class ValidadorXML
    {
        public int RetornoValidacao { get; private set; }
        public string RetornoValidacaoString { get; private set; }
        public int NumeroRetornoValidacaoTipoArquivo { get; private set; }
        public string RetornoValidacaoTipoArquivo { get; private set; }
        public string ArquivoSchema { get; private set; }
        private string l_Erro;

        /// <summary>
        /// Valida o arquivo informado baseado no arquvio de schema e na Url
        /// </summary>
        /// <param name="a_ArquivoXML">Arquivo a ser validado</param>
        /// <param name="a_ArquivoSchema">Arquivo XSD</param>
        /// <param name="a_URL">"http://www.portalfiscal.inf.br/nfe"</param>
        /// <returns>Resultado da validação: Branco validado com sucesso</returns>
        public string ValidarXML(string a_ArquivoXML, string a_ArquivoSchema, string a_URL)
        {
            bool l_ArquivoXML = File.Exists(a_ArquivoXML);
            bool l_ArquivoXSD = File.Exists(a_ArquivoSchema);

            //if (l_ArquivoXML && l_ArquivoXSD)
            if (l_ArquivoXSD)
            {
                StreamReader l_StreamReader = new StreamReader(a_ArquivoXML);
                XmlTextReader l_XmlTextReader = new XmlTextReader(l_StreamReader);
                
                XmlValidatingReader l_XmlValidatingReader = new XmlValidatingReader(l_XmlTextReader);

                // Criar um coleção de schema, adicionar o XSD para ela
                XmlSchemaCollection l_XmlSchemaCollection = new XmlSchemaCollection();
                l_XmlSchemaCollection.Add(a_URL, a_ArquivoSchema);

                // Adicionar a coleção de schema para o XmlValidatingReader
                l_XmlValidatingReader.Schemas.Add(l_XmlSchemaCollection);

                // Wire up the call back.  The ValidationEvent is fired when the
                // XmlValidatingReader hits an issue validating a section of the xml
                l_XmlValidatingReader.ValidationEventHandler += new ValidationEventHandler(reader_ValidationEventHandler);

                // Iterate through the xml document
                this.l_Erro = "";
                try
                {
                    while (l_XmlValidatingReader.Read()) { }
                }
                catch (Exception ex)
                {
                    this.l_Erro = this.l_Erro + "\r\n" + ex.Message;
                }

                l_XmlValidatingReader.Close();

                this.RetornoValidacao = 0;
                this.RetornoValidacaoString = "";
                if (l_Erro != "")
                {
                    this.RetornoValidacao = 1;
                    this.RetornoValidacaoString += this.l_Erro;
                }
            }
            else
            {
                //if (l_ArquivoXML == false)
                //{
                //    this.RetornoValidacao = 2;
                //    this.RetornoValidacaoString = "Arquivo XML não foi encontrato";
                //}
                //else 
                if (l_ArquivoXSD == false)
                {
                    this.RetornoValidacao = 3;
                    this.RetornoValidacaoString = "Arquivo XSD (schema) não foi encontrato";
                }
            }

            return this.RetornoValidacaoString;
        }

        public string ValidarXML(Byte[] a_ArquivoXML, string a_ArquivoSchema, string a_URL)
        {
            //bool l_ArquivoXML = File.Exists(a_ArquivoXML);
            bool l_ArquivoXSD = File.Exists(a_ArquivoSchema);

            //if (l_ArquivoXML && l_ArquivoXSD)
            if ( l_ArquivoXSD)
            {
                //StreamReader l_StreamReader = new StreamReader(a_ArquivoXML);
                //XmlTextReader l_XmlTextReader = new XmlTextReader(l_StreamReader);
                XmlTextReader l_XmlTextReader = new XmlTextReader(Geral.Instance.ObterStringMemoria(a_ArquivoXML));
                XmlValidatingReader l_XmlValidatingReader = new XmlValidatingReader(l_XmlTextReader);

                // Criar um coleção de schema, adicionar o XSD para ela
                XmlSchemaCollection l_XmlSchemaCollection = new XmlSchemaCollection();
                l_XmlSchemaCollection.Add(a_URL, a_ArquivoSchema);

                // Adicionar a coleção de schema para o XmlValidatingReader
                l_XmlValidatingReader.Schemas.Add(l_XmlSchemaCollection);

                // Wire up the call back.  The ValidationEvent is fired when the
                // XmlValidatingReader hits an issue validating a section of the xml
                l_XmlValidatingReader.ValidationEventHandler += new ValidationEventHandler(reader_ValidationEventHandler);

                // Iterate through the xml document
                this.l_Erro = "";
                try
                {
                    while (l_XmlValidatingReader.Read()) { }
                }
                catch (Exception ex)
                {
                    this.l_Erro = this.l_Erro + "\r\n" + ex.Message;
                }

                l_XmlValidatingReader.Close();

                this.RetornoValidacao = 0;
                this.RetornoValidacaoString = "";
                if (l_Erro != "")
                {
                    this.RetornoValidacao = 1;
                    this.RetornoValidacaoString += this.l_Erro;
                }
            }
            else
            {
                //if (l_ArquivoXML == false)
                //{
                //    this.RetornoValidacao = 2;
                //    this.RetornoValidacaoString = "Arquivo XML não foi encontrato";
                //}
                //else 
                if (l_ArquivoXSD == false)
                {
                    this.RetornoValidacao = 3;
                    this.RetornoValidacaoString = "Arquivo XSD (schema) não foi encontrato";
                }
            }

            return this.RetornoValidacaoString;
        }

        private void reader_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            this.l_Erro += "Linha: " + e.Exception.LineNumber + " Coluna: " + e.Exception.LinePosition + " Erro: " + e.Exception.Message + "\r\n";
        }

        /*
         * ==============================================================================
         * UNIMAKE - SOLUÇÕES CORPORATIVAS
         * ==============================================================================
         * Data.......: 31/07/2008
         * Autor......: Wandrey Mundin Ferreira
         * ------------------------------------------------------------------------------
         * Descrição..: Método responsável por retornar de que tipo é o arquivo XML.
         *              A principio o método retorna o tipo somente se é um XML for de:
         *              - Nota Fiscal Eletrônica
         *              - XML de envio de Lote de Notas Fiscais Eletrônicas
         *              - Cancelamento de Nota Fiscal Eletrônica
         *              - Inutilização de Numeração de Notas Fiscais Eletrônicas
         *              - Consulta da Situação da Nota Fiscal Eletrônica
         *              - Consulta do Recibo do Lote das Notas Fiscais Eletrônicas
         *              - Consulta do Status do Serviço da Nota Fiscal Eletrônica
         *              
         * ------------------------------------------------------------------------------
         * Definição..: TipoArquivoXML( string )
         * Parâmetros.: cRotaArqXML    - Rota e nome do arquivo XML que é
         *                               para ser retornado o tipo. Exemplo:
         *                               c:\unimake\EFiscal\envio\teste-nfe.xml
         *                        
         * ------------------------------------------------------------------------------
         * Retorno....: A propriedade this.nRetornoTipoArq vai receber um número 
         *              identificando se foi possível identificar o arquivo ou não
         *              1=Nota Fiscal Eletrônica
         *              2=XML de envio de Lote de Notas Fiscais Eletrônicas
         *              3=Cancelamento de Nota Fiscal Eletrônica
         *              4=Inutilização de Numeração de Notas Fiscais Eletrônicas
         *              5=Consulta da Situação da Nota Fiscal Eletrônica
         *              6=Consulta Recibo da Nota Fiscal Eletrônica
         *              7=Consulta do Status do Serviço da Nota Fiscal Eletrônica
         *              
         *              100=Arquivo XML não foi encontrato
         *              101=Arquivo não foi identificado
         * 
         * ------------------------------------------------------------------------------
         * Exemplos...:
         * 
         * oObj.TipoArquivoXML(@"c:\unimake\EFiscal\teste-nfe.xml")
         * if (oObj.nRetornoTipoArq == 1)
         * {
         *    MessageBox.Show("Nota Fiscal Eletrônica");
         * }
         * 
         * ------------------------------------------------------------------------------
         * Notas......:
         * 
         * ==============================================================================         
         */
        public void TipoArquivoXMLNFe(string a_ArquivoXML)
        {
            this.NumeroRetornoValidacaoTipoArquivo = 0;
            this.RetornoValidacaoTipoArquivo = "";
            this.ArquivoSchema = "";

            if (File.Exists(a_ArquivoXML))
            {
                //Carregar os dados do arquivo XML de configurações do EFiscal
                XmlTextReader l_XmlTextReader = new XmlTextReader(a_ArquivoXML);

                while (l_XmlTextReader.Read())
                {
                    if (l_XmlTextReader.NodeType == XmlNodeType.Element)
                    {
                        if (l_XmlTextReader.Name == "NFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 1;
                            this.RetornoValidacaoTipoArquivo = "XML de Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "nfe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "enviNFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 2;
                            this.RetornoValidacaoTipoArquivo = "XML de Envio de Lote de Notas Fiscais Eletrônicas";
                            this.ArquivoSchema = "enviNFe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "cancNFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 3;
                            this.RetornoValidacaoTipoArquivo = "XML de Cancelamento de Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "cancNFe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "inutNFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 4;
                            this.RetornoValidacaoTipoArquivo = "XML de Inutilização de Numerações de Notas Fiscais Eletrônicas";
                            this.ArquivoSchema = "inutNFe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "consSitNFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 5;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta da Situação da Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "consSitNFe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "consReciNFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 6;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta do Recibo do Lote de Notas Fiscais Eletrônicas";
                            this.ArquivoSchema = "consReciNfe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "consStatServ")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 7;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta da Situação do Serviço da Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "consStatServ_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "ConsCad")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 8;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta do Cadastro do Contribuinte";
                            this.ArquivoSchema = "consCad_v2.00.xsd";
                        }
                        if (this.NumeroRetornoValidacaoTipoArquivo != 0) //Arquivo já foi identificado
                        {
                            break;
                        }
                    }
                }
                l_XmlTextReader.Close();
            }
            else
            {
                this.NumeroRetornoValidacaoTipoArquivo = 100;
                this.RetornoValidacaoTipoArquivo = "Arquivo XML não foi encontrado";
            }

            if (this.NumeroRetornoValidacaoTipoArquivo == 0)
            {
                this.NumeroRetornoValidacaoTipoArquivo = 101;
                this.RetornoValidacaoTipoArquivo = "Não foi possível identificar o arquivo XML";
            }
        }

        public void TipoArquivoXMLCTe(string a_ArquivoXML)
        {
            this.NumeroRetornoValidacaoTipoArquivo = 0;
            this.RetornoValidacaoTipoArquivo = "";
            this.ArquivoSchema = "";

            if (File.Exists(a_ArquivoXML))
            {
                //Carregar os dados do arquivo XML de configurações do EFiscal
                XmlTextReader l_XmlTextReader = new XmlTextReader(a_ArquivoXML);

                while (l_XmlTextReader.Read())
                {
                    if (l_XmlTextReader.NodeType == XmlNodeType.Element)
                    {
                        if (l_XmlTextReader.Name == "CTe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 1;
                            this.RetornoValidacaoTipoArquivo = "XML de Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "CTe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "enviCTe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 2;
                            this.RetornoValidacaoTipoArquivo = "XML de Envio de Lote de Notas Fiscais Eletrônicas";
                            this.ArquivoSchema = "enviCTe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "cancCTe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 3;
                            this.RetornoValidacaoTipoArquivo = "XML de Cancelamento de Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "cancCTe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "inutCTe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 4;
                            this.RetornoValidacaoTipoArquivo = "XML de Inutilização de Numerações de Notas Fiscais Eletrônicas";
                            this.ArquivoSchema = "inutCTe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "consSitCTe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 5;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta da Situação da Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "consSitCTe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "consReciCTe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 6;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta do Recibo do Lote de Notas Fiscais Eletrônicas";
                            this.ArquivoSchema = "consReciCTe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "consStatServ")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 7;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta da Situação do Serviço da Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "consStatServ_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "ConsCad")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 8;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta do Cadastro do Contribuinte";
                            this.ArquivoSchema = "consCad_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "infEvento")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 9;
                            this.RetornoValidacaoTipoArquivo = "XML de ";
                            this.ArquivoSchema = "evCancCTe_v2.00.xsd";
                        }
                        //else if (l_XmlTextReader.Name == "")
                        //{
                        //    this.NumeroRetornoValidacaoTipoArquivo = 10;
                        //    this.RetornoValidacaoTipoArquivo = "XML de ";
                        //    this.ArquivoSchema = "evCCeCTe_v2.00.xsd";
                        //}
                        //else if (l_XmlTextReader.Name == "")
                        //{
                        //    this.NumeroRetornoValidacaoTipoArquivo = 11;
                        //    this.RetornoValidacaoTipoArquivo = "XML de ";
                        //    this.ArquivoSchema = "evEPECCTe_v2.00.xsd";
                        //}
                        //else if (l_XmlTextReader.Name == "")
                        //{
                        //    this.NumeroRetornoValidacaoTipoArquivo = 12;
                        //    this.RetornoValidacaoTipoArquivo = "XML de ";
                        //    this.ArquivoSchema = "eventoCTe_v2.00.xsd";
                        //}
                        //else if (l_XmlTextReader.Name == "")
                        //{
                        //    this.NumeroRetornoValidacaoTipoArquivo = 13;
                        //    this.RetornoValidacaoTipoArquivo = "XML de ";
                        //    this.ArquivoSchema = "evRegMultimodal_v2.00.xsd";
                        //}
                        if (this.NumeroRetornoValidacaoTipoArquivo != 0) //Arquivo já foi identificado
                        {
                            break;
                        }
                    }
                }
                l_XmlTextReader.Close();
            }
            else
            {
                this.NumeroRetornoValidacaoTipoArquivo = 100;
                this.RetornoValidacaoTipoArquivo = "Arquivo XML não foi encontrado";
            }

            if (this.NumeroRetornoValidacaoTipoArquivo == 0)
            {
                this.NumeroRetornoValidacaoTipoArquivo = 101;
                this.RetornoValidacaoTipoArquivo = "Não foi possível identificar o arquivo XML";
            }
        }

        public void TipoArquivoXMLMDFe(string a_ArquivoXML)
        {
            this.NumeroRetornoValidacaoTipoArquivo = 0;
            this.RetornoValidacaoTipoArquivo = "";
            this.ArquivoSchema = "";

            if (File.Exists(a_ArquivoXML))
            {
                //Carregar os dados do arquivo XML de configurações do EFiscal
                XmlTextReader l_XmlTextReader = new XmlTextReader(a_ArquivoXML);

                while (l_XmlTextReader.Read())
                {
                    if (l_XmlTextReader.NodeType == XmlNodeType.Element)
                    {
                        if (l_XmlTextReader.Name == "MDFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 1;
                            this.RetornoValidacaoTipoArquivo = "XML de Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "mdfe_v1.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "enviMDFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 2;
                            this.RetornoValidacaoTipoArquivo = "XML de Envio de Lote de Notas Fiscais Eletrônicas";
                            this.ArquivoSchema = "enviMDFe_v2.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "consSitMDFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 3;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta da Situação da Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "consSitMDFe_v1.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "consReciMDFe")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 4;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta do Recibo do Lote de Notas Fiscais Eletrônicas";
                            this.ArquivoSchema = "consReciMDFe_v1.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "consStatServ")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 5;
                            this.RetornoValidacaoTipoArquivo = "XML de Consulta da Situação do Serviço da Nota Fiscal Eletrônica";
                            this.ArquivoSchema = "consStatServ_v1.00.xsd";
                        }
                        else if (l_XmlTextReader.Name == "infEvento")
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = 6;
                            this.RetornoValidacaoTipoArquivo = "XML de ";
                            this.ArquivoSchema = "evCancMDFe_v1.00.xsd";
                        }

                        if (this.NumeroRetornoValidacaoTipoArquivo != 0) //Arquivo já foi identificado
                        {
                            break;
                        }
                    }
                }
                l_XmlTextReader.Close();
            }
            else
            {
                this.NumeroRetornoValidacaoTipoArquivo = 100;
                this.RetornoValidacaoTipoArquivo = "Arquivo XML não foi encontrado";
            }

            if (this.NumeroRetornoValidacaoTipoArquivo == 0)
            {
                this.NumeroRetornoValidacaoTipoArquivo = 101;
                this.RetornoValidacaoTipoArquivo = "Não foi possível identificar o arquivo XML";
            }
        }

        public void TipoArquivo(string a_ArquivoXML, string a_NomeTag, string a_Versao)
        {
            this.NumeroRetornoValidacaoTipoArquivo = 0;
            this.RetornoValidacaoTipoArquivo = "";
            this.ArquivoSchema = "";
            if (File.Exists(a_ArquivoXML))
            {
            //Carregar os dados do arquivo XML de configurações do EFiscal
            XmlTextReader l_XmlTextReader = new XmlTextReader(a_ArquivoXML);
            //XmlTextReader l_XmlTextReader = new XmlTextReader(Util.Instance.ObterXmlMemoria(a_ArquivoXML));

            while (l_XmlTextReader.Read())
            {
                if (l_XmlTextReader.NodeType == XmlNodeType.Element)
                {
                    if (l_XmlTextReader.Name == a_NomeTag)
                    {
                        this.NumeroRetornoValidacaoTipoArquivo =  Enums.Instance.ObterCodigoTipoArquivosCTe(a_NomeTag);
                        this.RetornoValidacaoTipoArquivo = Enums.Instance.ObterNomeTipoArquivosCTe(NumeroRetornoValidacaoTipoArquivo).Replace(a_NomeTag + "_", "").Replace("_", " ");
                        this.ArquivoSchema = a_NomeTag + "_v" + a_Versao + ".xsd";
                    }
                }
            }
            }
        }

        public void TipoArquivo(Byte[] a_ArquivoXML, string a_NomeTag, string a_Versao)
        {
            this.NumeroRetornoValidacaoTipoArquivo = 0;
            this.RetornoValidacaoTipoArquivo = "";
            this.ArquivoSchema = "";

            XmlTextReader l_XmlTextReader = new XmlTextReader(Geral.Instance.ObterStringMemoria(a_ArquivoXML));

            while (l_XmlTextReader.Read())
            {
                if (l_XmlTextReader.NodeType == XmlNodeType.Element)
                {
                    if (l_XmlTextReader.Name == a_NomeTag)
                    {
                        this.NumeroRetornoValidacaoTipoArquivo = Enums.Instance.ObterCodigoTipoArquivosCTe(a_NomeTag);
                        this.RetornoValidacaoTipoArquivo = Enums.Instance.ObterNomeTipoArquivosCTe(NumeroRetornoValidacaoTipoArquivo).Replace(a_NomeTag + "_", "").Replace("_", " ");
                        this.ArquivoSchema = a_NomeTag + "_v" + a_Versao + ".xsd";
                    }
                }
            }
        }

        public void TipoArquivoMDFe(string a_ArquivoXML, string a_NomeTag, string a_Versao)
        {
            this.NumeroRetornoValidacaoTipoArquivo = 0;
            this.RetornoValidacaoTipoArquivo = "";
            this.ArquivoSchema = "";
            if (File.Exists(a_ArquivoXML))
            {
                //Carregar os dados do arquivo XML de configurações do EFiscal
                XmlTextReader l_XmlTextReader = new XmlTextReader(a_ArquivoXML);
                //XmlTextReader l_XmlTextReader = new XmlTextReader(Util.Instance.ObterXmlMemoria(a_ArquivoXML));

                while (l_XmlTextReader.Read())
                {
                    if (l_XmlTextReader.NodeType == XmlNodeType.Element)
                    {
                        if (l_XmlTextReader.Name == a_NomeTag)
                        {
                            this.NumeroRetornoValidacaoTipoArquivo = Enums.Instance.ObterCodigoTipoArquivosMDFe(a_NomeTag);
                            this.RetornoValidacaoTipoArquivo = Enums.Instance.ObterNomeTipoArquivosMDFe(NumeroRetornoValidacaoTipoArquivo).Replace(a_NomeTag + "_", "").Replace("_", " ");
                            this.ArquivoSchema = a_NomeTag + "_v" + a_Versao + ".xsd";
                        }
                    }
                }
            }
        }

        public void TipoArquivoMDFe(Byte[] a_ArquivoXML, string a_NomeTag, string a_Versao)
        {
            this.NumeroRetornoValidacaoTipoArquivo = 0;
            this.RetornoValidacaoTipoArquivo = "";
            this.ArquivoSchema = "";

            XmlTextReader l_XmlTextReader = new XmlTextReader(Geral.Instance.ObterStringMemoria(a_ArquivoXML));

            while (l_XmlTextReader.Read())
            {
                if (l_XmlTextReader.NodeType == XmlNodeType.Element)
                {
                    if (l_XmlTextReader.Name == a_NomeTag)
                    {
                        this.NumeroRetornoValidacaoTipoArquivo = Enums.Instance.ObterCodigoTipoArquivosMDFe(a_NomeTag);
                        this.RetornoValidacaoTipoArquivo = Enums.Instance.ObterNomeTipoArquivosMDFe(NumeroRetornoValidacaoTipoArquivo).Replace(a_NomeTag + "_", "").Replace("_", " ");
                        this.ArquivoSchema = a_NomeTag + "_v" + a_Versao + ".xsd";
                    }
                }
            }
        }
    }
}
