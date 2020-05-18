using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
//using FWSCRM.XML;

namespace FWSCRM.Util
{
    public sealed class AssinaturaDigital
    {
        private XmlDocument g_XMLDoc;

        #region Singleton

        //Variaveis Privadas
        private static volatile AssinaturaDigital i_instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static AssinaturaDigital Instance
        {
            get
            {
                if (i_instance == null)
                {
                    lock (typeof(AssinaturaDigital))
                    {
                        if (i_instance == null)
                        {
                            i_instance = new AssinaturaDigital();
                        }
                    }
                }
                return i_instance;
            }
        }

        //Inicialização Privada
        private AssinaturaDigital()
        {
        }

        #endregion

        /// <summary>
        /// Recebe como conteúdo um valor inteiro com o resultado da assinatura, se deu algum errro, se foi assinado com sucesso, etc...
        /// </summary>
        public int ResultadoAssinatura { get; private set; }

        /// <summary>
        /// Recebe como conteúdo uma string com o resultado da assinatura, se deu algum errro, se foi assinado com sucesso, etc...
        /// </summary>
        public string ResultadoAssinaturaString { get; private set; }

        /// <summary>
        /// String do XML assinado
        /// </summary>
        public string XMLAssinado { get; private set; }

        /// <summary>
        /// O método assina digitalmente o arquivo XML passado por parâmetro e 
        /// grava o XML assinado com o mesmo nome, sobreponto o XML informado por parâmetro.
        /// Disponibiliza também uma propriedade com uma string do xml assinado (this.vXmlStringAssinado)
        /// </summary>
        /// <param name="pArqXMLAssinar">Nome do arquivo XML a ser assinado</param>
        /// <param name="pUri">URI (TAG) a ser assinada</param>
        /// <param name="pCertificado">Certificado a ser utilizado na assinatura</param>
        /// <remarks>
        /// Podemos pegar como retorno do método as seguintes propriedades:
        /// 
        /// - Atualiza a propriedade this.vXMLStringAssinado com a string de
        ///   xml já assinada
        ///   
        /// - Grava o XML sobreponto o informado para o método com o conteúdo
        ///   já assinado
        ///   
        /// - Atualiza as propriedades this.vResultado e 
        ///   this.vResultadoString com os seguintes valores:
        ///   
        ///   0 - Assinatura realizada com sucesso
        ///   1 - Erro: Problema ao acessar o certificado digital - %exceção%
        ///   2 - Problemas no certificado digital
        ///   3 - XML mal formado + %exceção%
        ///   4 - A tag de assinatura %pUri% não existe 
        ///   5 - A tag de assinatura %pUri% não é unica
        ///   6 - Erro ao assinar o documento - %exceção%
        ///   7 - Falha ao tentar abrir o arquivo XML - %exceção%
        /// </remarks>
        /// <by>Wandrey Mundin Ferreira</by>
        /// <date>04/06/2008</date>

        public void Assinar(Byte[] a_ArquivoXMLAssinar, string a_Uri, X509Certificate2 a_Certificado)
        {
            //Atualizar atributos de retorno com conteúdo padrão
            this.ResultadoAssinatura = 0;
            this.ResultadoAssinaturaString = "Assinatura realizada com sucesso";

            //StreamReader l_StreamReader = null;

            try
            {
                //Abrir o arquivo XML a ser assinado e ler o seu conteúdo
                //l_StreamReader =  File.OpenText(a_ArquivoXMLAssinar);
                string l_XMLString = Geral.Instance.ObterStringBytes(a_ArquivoXMLAssinar);//l_StreamReader.ReadToEnd();
                //l_StreamReader.Close();

                try
                {
                    // Verifica o certificado a ser utilizado na assinatura
                    string l_XNome = "";
                    if (a_Certificado != null)
                    {
                        l_XNome = a_Certificado.Subject.ToString();
                    }

                    X509Certificate2 l_X509Certificate2 = new X509Certificate2();
                    X509Store l_X509Store = new X509Store("MY", StoreLocation.CurrentUser);
                    l_X509Store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    X509Certificate2Collection l_X509Certificate2Collection = (X509Certificate2Collection)l_X509Store.Certificates;
                    X509Certificate2Collection l_X509Certificate2Collection1 = (X509Certificate2Collection)l_X509Certificate2Collection.Find(X509FindType.FindBySubjectDistinguishedName, l_XNome, false);

                    if (l_X509Certificate2Collection1.Count == 0)
                    {
                        this.ResultadoAssinatura = 2;
                        this.ResultadoAssinaturaString = "Problemas no certificado digital";
                    }
                    else
                    {
                        // certificado ok
                        l_X509Certificate2 = l_X509Certificate2Collection1[0];
                        string l_KeyAlgorithm;
                        l_KeyAlgorithm = l_X509Certificate2.GetKeyAlgorithm().ToString();

                        // Create a new XML document.
                        XmlDocument l_XmlDocument = new XmlDocument();

                        // Format the document to ignore white spaces.
                        l_XmlDocument.PreserveWhitespace = false;

                        // Load the passed XML file using it’s name.
                        try
                        {
                            l_XmlDocument.LoadXml(l_XMLString);

                            // Verifica se a tag a ser assinada existe é única
                            int l_QtdeRefUri = l_XmlDocument.GetElementsByTagName(a_Uri).Count;

                            if (l_QtdeRefUri == 0)
                            {
                                // a URI indicada não existe
                                this.ResultadoAssinatura = 4;
                                this.ResultadoAssinaturaString = "A tag de assinatura " + a_Uri.Trim() + " não existe";
                            }
                            // Exsiste mais de uma tag a ser assinada
                            else
                            {
                                if (l_QtdeRefUri > 1)
                                {
                                    // existe mais de uma URI indicada
                                    this.ResultadoAssinatura = 5;
                                    this.ResultadoAssinaturaString = "A tag de assinatura " + a_Uri.Trim() + " não é unica";
                                }
                                else
                                {
                                    try
                                    {
                                        // Create a SignedXml object.
                                        SignedXml l_SignedXml = new SignedXml(l_XmlDocument);

                                        // Add the key to the SignedXml document
                                        l_SignedXml.SigningKey = l_X509Certificate2.PrivateKey;

                                        // Create a reference to be signed
                                        Reference l_Reference = new Reference();

                                        // pega o uri que deve ser assinada
                                        XmlAttributeCollection l_XmlAttributeCollection = l_XmlDocument.GetElementsByTagName(a_Uri).Item(0).Attributes;
                                        foreach (XmlAttribute l_XmlAttribute in l_XmlAttributeCollection)
                                        {
                                            if (l_XmlAttribute.Name == "Id")
                                            {
                                                l_Reference.Uri = "#" + l_XmlAttribute.InnerText;
                                            }
                                        }

                                        // Add an enveloped transformation to the reference.
                                        XmlDsigEnvelopedSignatureTransform l_XmlDsigEnvelopedSignatureTransform = new XmlDsigEnvelopedSignatureTransform();
                                        l_Reference.AddTransform(l_XmlDsigEnvelopedSignatureTransform);

                                        XmlDsigC14NTransform l_XmlDsigC14NTransform = new XmlDsigC14NTransform();
                                        l_Reference.AddTransform(l_XmlDsigC14NTransform);

                                        // Add the reference to the SignedXml object.
                                        l_SignedXml.AddReference(l_Reference);

                                        // Create a new KeyInfo object
                                        KeyInfo l_keyInfo = new KeyInfo();

                                        // Load the certificate into a KeyInfoX509Data object
                                        // and add it to the KeyInfo object.
                                        l_keyInfo.AddClause(new KeyInfoX509Data(l_X509Certificate2));

                                        // Add the KeyInfo object to the SignedXml object.
                                        l_SignedXml.KeyInfo = l_keyInfo;
                                        l_SignedXml.ComputeSignature();

                                        // Get the XML representation of the signature and save
                                        // it to an XmlElement object.
                                        XmlElement xmlDigitalSignature = l_SignedXml.GetXml();

                                        // Gravar o elemento no documento XML
                                        l_XmlDocument.DocumentElement.AppendChild(l_XmlDocument.ImportNode(xmlDigitalSignature, true));
                                        g_XMLDoc = new XmlDocument();
                                        g_XMLDoc.PreserveWhitespace = true;
                                        g_XMLDoc = l_XmlDocument;

                                        // Atualizar a string do XML já assinada
                                        this.XMLAssinado = g_XMLDoc.OuterXml;

                                        // Gravar o XML no HD
                                        //StreamWriter l_StreamWriter = File.CreateText(a_ArquivoXMLAssinar);
                                        //l_StreamWriter.Write(this.XMLAssinado);
                                        //l_StreamWriter.Close();
                                        //XML.Instance.XMLCTe = Geral.Instance.ObterBytesString(this.XMLAssinado);
                                        XML.XML.Instance.XMLCTe = Geral.Instance.ObterBytesString(this.XMLAssinado);
                                        
                                    }
                                    catch (Exception caught)
                                    {
                                        this.ResultadoAssinatura = 6;
                                        this.ResultadoAssinaturaString = "Erro ao assinar o documento - " + caught.Message;
                                    }
                                }
                            }
                        }
                        catch (Exception caught)
                        {
                            this.ResultadoAssinatura = 3;
                            this.ResultadoAssinaturaString = "XML mal formado - " + caught.Message;
                        }
                    }
                }
                catch (Exception caught)
                {
                    this.ResultadoAssinatura = 1;
                    this.ResultadoAssinaturaString = "Problema ao acessar o certificado digital - " + caught.Message;
                }
            }
            catch (Exception ex)
            {
                this.ResultadoAssinatura = 1;
                this.ResultadoAssinaturaString = "Falha ao tentar abrir/ler o arquivo XML - " + ex.Message;
            }
            finally
            {
                //l_StreamReader.Close();
            }
        }

        public String Assinar(string a_ArquivoXMLAssinar, string a_Uri, X509Certificate2 a_Certificado)
        {
            //Atualizar atributos de retorno com conteúdo padrão
            this.ResultadoAssinatura = 0;
            this.ResultadoAssinaturaString = "Assinatura realizada com sucesso";

            //StreamReader l_StreamReader = null;

            try
            {
                //Abrir o arquivo XML a ser assinado e ler o seu conteúdo

                //l_StreamReader =  File.OpenText(a_ArquivoXMLAssinar);
                //string l_XMLString = l_StreamReader.ReadToEnd();
                string l_XMLString = a_ArquivoXMLAssinar;

                //l_StreamReader.Close();

                try
                {
                    // Verifica o certificado a ser utilizado na assinatura
                    string l_XNome = "";
                    if (a_Certificado != null)
                    {
                        l_XNome = a_Certificado.Subject.ToString();
                    }

                    X509Certificate2 l_X509Certificate2 = new X509Certificate2();
                    X509Store l_X509Store = new X509Store("MY", StoreLocation.CurrentUser);
                    l_X509Store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    X509Certificate2Collection l_X509Certificate2Collection = (X509Certificate2Collection)l_X509Store.Certificates;
                    X509Certificate2Collection l_X509Certificate2Collection1 = (X509Certificate2Collection)l_X509Certificate2Collection.Find(X509FindType.FindBySubjectDistinguishedName, l_XNome, false);

                    if (l_X509Certificate2Collection1.Count == 0)
                    {
                        this.ResultadoAssinatura = 2;
                        this.ResultadoAssinaturaString = "Problemas no certificado digital";
                        return this.XMLAssinado;
                    }
                    else
                    {
                        // certificado ok
                        l_X509Certificate2 = l_X509Certificate2Collection1[0];
                        string l_KeyAlgorithm;
                        l_KeyAlgorithm = l_X509Certificate2.GetKeyAlgorithm().ToString();

                        // Create a new XML document.
                        XmlDocument l_XmlDocument = new XmlDocument();

                        // Format the document to ignore white spaces.
                        l_XmlDocument.PreserveWhitespace = false;

                        // Load the passed XML file using it’s name.
                        try
                        {
                            l_XmlDocument.LoadXml(l_XMLString);

                            // Verifica se a tag a ser assinada existe é única
                            int l_QtdeRefUri = l_XmlDocument.GetElementsByTagName(a_Uri).Count;

                            if (l_QtdeRefUri == 0)
                            {
                                // a URI indicada não existe
                                this.ResultadoAssinatura = 4;
                                this.ResultadoAssinaturaString = "A tag de assinatura " + a_Uri.Trim() + " não existe";
                                return this.XMLAssinado;
                            }
                            // Exsiste mais de uma tag a ser assinada
                            else
                            {
                                if (l_QtdeRefUri > 1)
                                {
                                    // existe mais de uma URI indicada
                                    this.ResultadoAssinatura = 5;
                                    this.ResultadoAssinaturaString = "A tag de assinatura " + a_Uri.Trim() + " não é unica";
                                    return this.XMLAssinado;
                                }
                                else
                                {
                                    try
                                    {
                                        // Create a SignedXml object.
                                        SignedXml l_SignedXml = new SignedXml(l_XmlDocument);

                                        // Add the key to the SignedXml document
                                        l_SignedXml.SigningKey = l_X509Certificate2.PrivateKey;

                                        // Create a reference to be signed
                                        Reference l_Reference = new Reference();

                                        // pega o uri que deve ser assinada
                                        XmlAttributeCollection l_XmlAttributeCollection = l_XmlDocument.GetElementsByTagName(a_Uri).Item(0).Attributes;
                                        foreach (XmlAttribute l_XmlAttribute in l_XmlAttributeCollection)
                                        {
                                            if (l_XmlAttribute.Name == "Id")
                                            {
                                                l_Reference.Uri = "#" + l_XmlAttribute.InnerText;
                                            }
                                        }
                                        //l_Reference.Uri = "";
                                        // Add an enveloped transformation to the reference.
                                        XmlDsigEnvelopedSignatureTransform l_XmlDsigEnvelopedSignatureTransform = new XmlDsigEnvelopedSignatureTransform();
                                        l_Reference.AddTransform(l_XmlDsigEnvelopedSignatureTransform);

                                        XmlDsigC14NTransform l_XmlDsigC14NTransform = new XmlDsigC14NTransform();
                                        l_Reference.AddTransform(l_XmlDsigC14NTransform);

                                        // Add the reference to the SignedXml object.
                                        l_SignedXml.AddReference(l_Reference);

                                        // Create a new KeyInfo object
                                        KeyInfo l_keyInfo = new KeyInfo();

                                        // Load the certificate into a KeyInfoX509Data object
                                        // and add it to the KeyInfo object.
                                        l_keyInfo.AddClause(new KeyInfoX509Data(l_X509Certificate2));

                                        // Add the KeyInfo object to the SignedXml object.
                                        l_SignedXml.KeyInfo = l_keyInfo;
                                        l_SignedXml.ComputeSignature();

                                        // Get the XML representation of the signature and save
                                        // it to an XmlElement object.
                                        XmlElement xmlDigitalSignature = l_SignedXml.GetXml();

                                        // Gravar o elemento no documento XML
                                        l_XmlDocument.DocumentElement.AppendChild(l_XmlDocument.ImportNode(xmlDigitalSignature, true));
                                        g_XMLDoc = new XmlDocument();
                                        g_XMLDoc.PreserveWhitespace = true;
                                        g_XMLDoc = l_XmlDocument;

                                        // Atualizar a string do XML já assinada
                                        this.XMLAssinado = g_XMLDoc.OuterXml;

                                        // Gravar o XML no HD
                                        //StreamWriter l_StreamWriter = File.CreateText(a_ArquivoXMLAssinar);
                                        //l_StreamWriter.Write(this.XMLAssinado);
                                        //l_StreamWriter.Close();
                                        //XML.Instance.XMLCTe = Util.Instance.ObterXmlBytes(this.XMLAssinado);
                                        return this.XMLAssinado;
                                    }
                                    catch (Exception caught)
                                    {
                                        this.ResultadoAssinatura = 6;
                                        this.ResultadoAssinaturaString = "Erro ao assinar o documento - " + caught.Message;
                                        return this.XMLAssinado;
                                    }
                                }
                            }
                        }
                        catch (Exception caught)
                        {
                            this.ResultadoAssinatura = 3;
                            this.ResultadoAssinaturaString = "XML mal formado - " + caught.Message;
                            return this.XMLAssinado;
                        }
                    }
                }
                catch (Exception caught)
                {
                    this.ResultadoAssinatura = 1;
                    this.ResultadoAssinaturaString = "Problema ao acessar o certificado digital - " + caught.Message;
                    return this.XMLAssinado;
                }
            }
            catch (Exception ex)
            {
                this.ResultadoAssinatura = 1;
                this.ResultadoAssinaturaString = "Falha ao tentar abrir/ler o arquivo XML - " + ex.Message;
                return this.XMLAssinado;
            }
            finally
            {
                //l_StreamReader.Close();

            }
        }

        public string SignElementDSF(string mensagemXML, X509Certificate2 certificado, string TAGFinal)
        {
            XmlDocument xmlDoc = new XmlDocument();
            RSACryptoServiceProvider Key = new RSACryptoServiceProvider();
            SignedXml SignedDocument = default(SignedXml);
            KeyInfo keyInfo = new KeyInfo();
            xmlDoc.LoadXml(mensagemXML);

            XmlNodeList XMLNode = xmlDoc.GetElementsByTagName("OT");

            //Retira chave privada ligada ao certificado
            Key = (RSACryptoServiceProvider)certificado.PrivateKey;

            //Adiciona Certificado ao Key Info
            keyInfo.AddClause(new KeyInfoX509Data(certificado));
            SignedDocument = new SignedXml(xmlDoc);

            //Seta chaves
            SignedDocument.SigningKey = Key;
            SignedDocument.KeyInfo = keyInfo;

            // Cria referencia
            Reference reference = new Reference();
            reference.Uri = string.Empty;

            // Adiciona transformacao a referencia
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform(false));

            // Adiciona referencia ao xml
            SignedDocument.AddReference(reference);

            // Calcula Assinatura
            SignedDocument.ComputeSignature();

            // Pega representação da assinatura
            XmlElement xmlDigitalSignature = SignedDocument.GetXml();


            // Adiciona ao doc XML
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

            #region "Pega o Signature"

            string XMLAssinado = xmlDoc.InnerXml;

            int PosiçãoSignature = XMLAssinado.IndexOf("<Signature");

            int PosicaoFinalSignature = XMLAssinado.IndexOf("</Signature>") + 12;

            int Quantidade = PosicaoFinalSignature - PosiçãoSignature;

            string Signature = XMLAssinado.Substring(PosiçãoSignature, Quantidade);

            #endregion

            string XMLFinalAssinado;

            if (TAGFinal != "")
            {
                XMLFinalAssinado = mensagemXML.Replace(TAGFinal, TAGFinal + Signature);
            }
            else
            {
                XMLFinalAssinado = xmlDoc.InnerXml;
            }

            return XMLFinalAssinado;
        }

        public X509Certificate2 FindCertificateBySubjectName(string subjectName)
        {
            //Repositório de certificados
            //Utilizar o repositório passado como parâmetro
            X509Store st = new X509Store(StoreLocation.LocalMachine);

            try
            {
                //Utilizar somente leitura
                st.Open(OpenFlags.ReadOnly);

                //Encontrar o certificado na coleção
                X509Certificate2Collection collection = st.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

                if (collection.Count > 0)
                    return collection[0];   //Retornar o certificado
                else
                {
                    collection = st.Certificates.Find(X509FindType.FindBySubjectName, subjectName, false);
                    if (collection.Count > 0)
                        return collection[0];   //Retornar o certificado
                }
            }
            finally
            {
                //Liberar os recursos
                if (st != null)
                    try
                    {
                        st.Close();
                    }
                    catch
                    {
                        st = null;
                    }
            }
            //Deverá retornar null se não encontrar o certificado na coleção
            return null;
        }

    }
}
