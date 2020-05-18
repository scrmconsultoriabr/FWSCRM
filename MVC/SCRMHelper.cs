using FWSCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FWSCRM.MVC
{
    public static class HtmlHelperExtension
    {
        private static String Menu { get; set; }

        public static MvcHtmlString LinkVoltar(
            this HtmlHelper html,
            string idLink,
            string textoLink = "Voltar")
        {
            string strLink = String.Format(
        "<a id=\"{0}\" href=\"javascript:history.go(-1);\">{1}</a>",
                idLink, textoLink);
            return new MvcHtmlString(strLink);
        }

        /// <summary>
        /// Helper para geração do menu dinâmico
        /// </summary>
        /// <param name="a_Menus">Menus retornados do banco de dados</param>
        /// <param name="a_ClasseLi">css para formatação das chamadas dos sub-menus</param>
        /// <param name="a_ClasseUL">css para formatação dos sub-menus</param>
        /// <returns>Menus formatados</returns>
        public static MvcHtmlString CriaMenu(List<Menu> a_Menus, String a_ClasseLi, String a_ClasseUL)
        {
            //Variável de trabalho para gerar o menu
            Menu = string.Empty;
            //Contador para percorrer os pais principais dos menus
            int l_IdPai = -1;

            //Filtra sub-menus baseado no pai principal - 0
            var l_MenusPai = a_Menus.Where(m => m.IdPai == 0).ToList();

            //Percorre lista dos pais principais, para geraçãos dos filhos e do menu geral
            foreach (var l_Menu in l_MenusPai)
            {
                //Incrementa contador do pai para pegar a primeira relação de menu pai
                l_IdPai++;

                //Obtem chamada do menu principal
                string l_Li = GeraMenuPai(a_ClasseLi, a_Menus[l_IdPai]);

                //Gera sub-menus do menu principal
                BuildMenu(l_Menu.MenuItems, l_Li, a_ClasseLi, a_ClasseUL);
            }

            //Retorna menu completo
            return new MvcHtmlString(Menu);
        }

        /// <summary>
        /// Listga com os dados do sub-menu filho do menu principal
        /// </summary>
        /// <param name="a_Menu">Lista com os sub-menus</param>
        /// <param name="a_Li">Variável de trabalho para geração dos sub-menus</param>
        /// <param name="a_ClasseLi">Css de formatação da chamada do sub-menu</param>
        /// <param name="a_ClasseUL">Css formatação dos sub-menus</param>
        private static void BuildMenu(List<Menu> a_Menu, String a_Li, String a_ClasseLi, String a_ClasseUL)
        {
            //Cria tag dos sub-menus
            a_Li += "<ul class=\"" + a_ClasseUL + "\">" + Environment.NewLine;

            //Percorre lista de menus para adiconar os sub-menus
            foreach (Menu l_Item in a_Menu)
            {
                //Se tem filhos gera recursivamente todos os filhos do sub-menu
                if (l_Item.MenuItems.Count > 0)
                {
                    // Cria cabeçalho do menu pai dos filhos do sub-menu
                    a_Li += GeraMenuPai(a_ClasseLi, l_Item);

                    //Dispara a rotina novamente para criar os sub-menus filhos
                    BuildMenu(l_Item.MenuItems, a_Li, a_ClasseLi, a_ClasseUL);

                    //Limpa variável pai dos filhos do sub-Menu 
                    a_Li = string.Empty;
                }
                else
                {
                    //Adiciona os sub-menus
                    a_Li += " <li><a id = \"" + l_Item.IdMenu + "\" href = \"" + l_Item.Url + "\"><i class=\"fa fa-angle-double-right\"></i>" + l_Item.NomeMenu + "</a></li>" + Environment.NewLine;
                }
            }

            //Adiciona tag´s do menu a variávei global principal
            Menu += a_Li + "</ul>" + Environment.NewLine + "</li>" + Environment.NewLine;

            //Limpa variável de trabnalho, pois já foi adicionada a variável global Menu.
            a_Li = string.Empty;
        }

        /// <summary>
        /// Gera chamada do pai do sub-menu
        /// </summary>
        /// <param name="a_ClasseLi">Classe para formatar a chamada</param>
        /// <param name="a_Itens">Estrutura com os dados da chamada do sub-menu</param>
        /// <returns>Html com a chamada do sub-menu</returns>
        private static string GeraMenuPai(string a_ClasseLi, Menu a_Itens)
        {
            string a_Li = "<li class=\"" + a_ClasseLi + "\">" + Environment.NewLine;
            a_Li += "<a href = \"" + a_Itens.Url + "\">" + Environment.NewLine;
            a_Li += "<i class=\"fa fa-trello\"></i><span id =\"id_" + a_Itens.NomeMenu + "\"> " + a_Itens.NomeMenu + "</span>" + Environment.NewLine;
            a_Li += "<i class=\"fa fa-angle-left pull-right\"></i>" + Environment.NewLine;
            a_Li += "</a>" + Environment.NewLine;
            return a_Li;
        }     
    }
}