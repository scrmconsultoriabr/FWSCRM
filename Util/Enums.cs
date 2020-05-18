using System;
using System.Collections.Generic;

namespace FWSCRM.Util
{
    public sealed class Enums
    {
        #region Singleton

        //Variaveis Privadas
        private static volatile Enums i_instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static Enums Instance
        {
            get
            {
                if (i_instance == null)
                {
                    lock (typeof(Enums))
                    {
                        if (i_instance == null)
                        {
                            i_instance = new Enums();
                        }
                    }
                }
                return i_instance;
            }
        }

        //Inicialização Privada
        private Enums()
        {
        }

        #endregion

        List<Object> ListaEnums = new List<Object>();
        public enum Municipio
        {
            //Salvador = 292740005
            Salvador = 2927408,
            SimoesFilho = 2930709
        }

        public enum TipoArquivosNFe
        {
            NFe_XML_de_Nota_Fiscal_Eletrônica = 1,
            enviNFe_XML_de_Envio_de_Lote_de_Notas_Fiscais_Eletrônicas = 2,
            cancNFe_XML_de_Cancelamento_de_Nota_Fiscal_Eletrônica = 3,
            inutNFe_XML_de_Inutilização_de_Numerações_de_Notas_Fiscais_Eletrônicas = 4,
            consSitNFe_XML_de_Consulta_da_Situação_da_Nota_Fiscal_Eletrônica = 5,
            consReciNFe_XML_de_Consulta_do_Recibo_do_Lote_de_Notas_Fiscais_Eletrônicas = 6,
            consStatServ_XML_de_Consulta_da_Situação_do_Serviço_da_Nota_Fiscal_Eletrônica = 7,
            ConsCad_XML_de_Consulta_do_Cadastro_do_Contribuinte = 8
        }

        public enum TipoArquivosCTe
        {
            CTe_XML_de_Conhecimeto_Fiscal_Eletrônico = 1,
            enviCTe_XML_de_Envio_de_Lote_de_Conhecimetos_Fiscais_Eletrônicos = 2,
            cancCTe_XML_de_Cancelamento_de_Conhecimeto_Fiscal_Eletrônico = 3,
            inutCTe_XML_de_Inutilização_de_Numerações_de_Conhecimetos_Fiscais_Eletrônicos = 4,
            consSitCTe_XML_de_Consulta_da_Situação_da_Conhecimeto_Fiscal_Eletrônico = 5,
            consReciCTe_XML_de_Consulta_do_Recibo_do_Lote_de_Conhecimetos_Fiscais_Eletrônicos = 6,
            consStatServCte_XML_de_Consulta_da_Situação_do_Serviço_da_Conhecimeto_Fiscal_Eletrônico = 7,
            ConsCad_XML_de_Consulta_do_Cadastro_do_Contribuinte = 8,
            eventoCTe_CTe_XML_de_Evento_Cancelamento_de_Conhecimeto_Fiscal_Eletrônico = 9
        }

        public enum TipoArquivosMDFe
        {
            MDFe_XML_de_Manifesto_Fiscal_Eletrônico = 1,
            enviMDFe_XML_de_Envio_de_Lote_de_Manifestos_Fiscais_Eletrônicos = 2,
            consSitMDFe_XML_de_Consulta_da_Situação_da_Manifesto_Fiscal_Eletrônico = 3,
            consReciMDFe_XML_de_Consulta_do_Recibo_do_Lote_de_Manifestos_Fiscais_Eletrônicos = 4,
            consStatServMDFe_XML_de_Consulta_da_Situação_do_Serviço_da_Manifesto_Fiscal_Eletrônico = 5,
            eventoMDFe_MDFe_XML_de_Evento_Cancelamento_de_Manifestoo_Fiscal_Eletrônico = 6
        }

        public enum ServicosNFe
        {
            CancelarNFe,
            InutilizarNumerosNFe,
            PedidoConsultaSituacaoNFe,
            PedidoConsultaStatusServicoNFe,
            PedidoSituacaoLoteNFe,
            ConsultaCadastroContribuinte,
            ConsultaInformacoesUniNFe,
            AlterarConfiguracoesUniNFe,
            AssinarNFePastaEnvio,
            AssinarNFePastaEnvioEmLote,
            MontarLoteUmaNFe,
            MontarLoteVariasNFe,
            EnviarLoteNfe,
            ValidarAssinar,
            ConverterTXTparaXML,
            GerarChaveNFe,
            EmProcessamento
        }

        public enum ServicosCTe
        {
            CancelarCTe,
            InutilizarNumerosCTe,
            PedidoConsultaSituacaoCTe,
            PedidoConsultaStatusServicoCTe,
            PedidoSituacaoLoteCTe,
            ConsultaCadastroContribuinte,
            ConsultaInformacoesUniCTe,
            AlterarConfiguracoesUniCTe,
            AssinarCTePastaEnvio,
            AssinarCTePastaEnvioEmLote,
            MontarLoteUmaCTe,
            MontarLoteVariasCTe,
            EnviarLoteCTe,
            ValidarAssinar,
            ConverterTXTparaXML,
            GerarChaveCTe,
            EmProcessamento
        }

        public enum UFs
        {
            AC = 12,
            AM = 13,
            AP = 16,
            RO = 11,
            RR = 14,
            PA = 15,
            TO = 17,
            AL = 27,
            BA = 29,
            CE = 23,
            MA = 21,
            PB = 25,
            PE = 26,
            PI = 22,
            RN = 24,
            SE = 28,
            DF = 53,
            GO = 52,
            MS = 50,
            MT = 51,
            ES = 32,
            MG = 31,
            RJ = 33,
            SP = 35,
            PR = 41,
            RS = 43,
            SC = 42,
            EX = 99
        }     

        #region Variaves

        //public TipoArquivosNFe TiposArquivosNFe { get; }
        //public TipoArquivosCTe TiposArquivosCTe { get; }
        //public UFs UFEstados { get; }

        #endregion

        public void Enumns()
        {

        }

        #region Métodos

        public string ObterNomeEnumarator(int a_CodigoEnum)
        {
            CarregarListEnums();
            string l_NomeEnumarator = string.Empty;
            foreach (Object l_Enum in ListaEnums)
            {
                foreach (int l_CodigoEnum in Enum.GetValues(l_Enum.GetType()))
                {
                    //if (l_CodigoEnum.ToString() == a_CodigoEnum.ToString().Trim())
                    if (l_CodigoEnum.ToString().Trim().Contains(a_CodigoEnum.ToString()))
                    {
                        //Pega a Descrição do enumarator que tem o valor da variável a_CodigoUF
                        l_NomeEnumarator = Enum.GetName(typeof(UFs), a_CodigoEnum);
                        break;
                    }
                }
            }

            return l_NomeEnumarator;
        }

        public int ObterCodigoTipoArquivosMDFe(string a_NomeEnumarator)
        {
            string l_CodigoEnumarator = string.Empty;
            foreach (string l_NomeEnum in Enum.GetNames(typeof(TipoArquivosMDFe)))
            {
                if (l_NomeEnum.ToUpper().Trim().Contains(a_NomeEnumarator.ToUpper().Trim()))
                {
                    l_CodigoEnumarator = ((int)Enum.Parse(typeof(TipoArquivosMDFe), l_NomeEnum)).ToString();
                    break;
                }
            }
            return int.Parse(l_CodigoEnumarator);
        }

        public string ObterNomeTipoArquivosMDFe(int a_CodigoEnum)
        {

            string l_NomeEnumarator = string.Empty;

            foreach (int l_CodigoEnum in Enum.GetValues(typeof(TipoArquivosMDFe)))
            {
                if (l_CodigoEnum.ToString().Trim().Contains(a_CodigoEnum.ToString()))
                {
                    l_NomeEnumarator = Enum.GetName(typeof(TipoArquivosMDFe), a_CodigoEnum);
                    break;
                }
            }
            return l_NomeEnumarator;
        }

        public int ObterCodigoTipoArquivosCTe(string a_NomeEnumarator)
        {
            string l_CodigoEnumarator = string.Empty;
            foreach (string l_NomeEnum in Enum.GetNames(typeof(TipoArquivosCTe)))
            {
                if (l_NomeEnum.ToUpper().Trim().Contains(a_NomeEnumarator.ToUpper().Trim()))
                {
                    l_CodigoEnumarator = ((int)Enum.Parse(typeof(TipoArquivosCTe), l_NomeEnum)).ToString();
                    break;
                }
            }
            return int.Parse(l_CodigoEnumarator);
        }

        public string ObterNomeTipoArquivosCTe(int a_CodigoEnum)
        {

            string l_NomeEnumarator = string.Empty;

            foreach (int l_CodigoEnum in Enum.GetValues(typeof(TipoArquivosCTe)))
            {
                if (l_CodigoEnum.ToString().Trim().Contains(a_CodigoEnum.ToString()))
                {
                    l_NomeEnumarator = Enum.GetName(typeof(TipoArquivosCTe), a_CodigoEnum);
                    break;
                }
            }
            return l_NomeEnumarator;
        }

        //public int ObterCodigoEnumarator(string a_NomeEnumarator)
        //{
        //    CarregarListEnums();
        //    string l_NomeEnumarator = string.Empty;
        //    foreach (Object l_Enum in ListaEnums)
        //    {
        //        try
        //        {
        //            foreach (string l_NomeEnum in Enum.GetNames(l_Enum.GetType()))
        //            {
        //                //if (l_NomeEnum.ToUpper() == a_NomeEnumarator.ToUpper().Trim())
        //                if (l_NomeEnum.ToUpper().Trim().Contains(a_NomeEnumarator.ToUpper().Trim()))
        //                {
        //                    l_NomeEnumarator = ((int)Enum.Parse(l_Enum.GetType(), l_NomeEnum)).ToString();
        //                    break;
        //                }
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            string l_erro = ex.Message;
        //        }
        //    }

        //    return int.Parse(l_NomeEnumarator);
        //}

        public String ObterCodigoEnumarator(Object a_Enum)
        {
            string l_ValorEnumarator = ((int)a_Enum).ToString();
            return l_ValorEnumarator;
        }

        /*
        public int ObterCodigoEnumarator(Municipio a_NomeEnumarator)
        {
            CarregarListEnums();
            string l_NomeEnumarator = string.Empty;
            foreach (Object l_Enum in ListaEnums)
            {
                try
                {
                    foreach (string l_NomeEnum in Enum.GetNames(l_Enum.GetType()))
                    {
                        //if (l_NomeEnum.ToUpper() == a_NomeEnumarator.ToUpper().Trim())
                        if (l_NomeEnum.ToUpper().Trim().Contains(a_NomeEnumarator.ToString().ToUpper().Trim()))
                        {
                            l_NomeEnumarator = ((int)Enum.Parse(l_Enum.GetType(), l_NomeEnum)).ToString();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string l_erro = ex.Message;
                }
            }

            return int.Parse(l_NomeEnumarator);
        }
        */
        private void CarregarListEnums()
        {
            ListaEnums.Add(typeof(TipoArquivosNFe));
            ListaEnums.Add(typeof(TipoArquivosCTe));
            ListaEnums.Add(typeof(TipoArquivosMDFe));

            ListaEnums.Add(typeof(ServicosNFe));
            ListaEnums.Add(typeof(ServicosCTe));

            ListaEnums.Add(typeof(UFs));
        }

        #endregion

    }
}
