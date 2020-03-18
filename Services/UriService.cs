using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using UriWebApi.Models;
using UriWebApi.WebCrawler;

namespace UriWebApi.Services
{
    public class UriService
    {
        private ExtratorHtml extratorHtml = new ExtratorHtml();
        private readonly string linkUriRankingUnasp = @"https://www.urionlinejudge.com.br/judge/pt/users/university/unasp";
        private readonly string linkUriRankingPaginadoUnasp = @"https://www.urionlinejudge.com.br/judge/pt/users/university/unasp?page={0}&direction=DESC";

        /// <summary>
        /// Busca a queryString no ranking da faculdade escolhida
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public object GetRanking(string queryString)
        {
            try
            {
                List<RankingDto> listaRanking = new List<RankingDto>();
                List<string> query = TratarQueryString(queryString.Trim());
                int posicao = 1;
                int totalPaginas = GetTotalPaginas();

                for (int numPagina = 1; numPagina <= totalPaginas; numPagina++)
                {
                    string paginaHtml = extratorHtml.CarregarPaginaHtml(linkUriRankingPaginadoUnasp, numPagina.ToString());

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(paginaHtml);
                    paginaHtml = string.Empty;

                    foreach (var node in document.DocumentNode.SelectNodes("//table/tbody/tr"))
                    {
                        if (node.ChildNodes.Count >= 7 && !string.IsNullOrWhiteSpace(node.ChildNodes[2].InnerText))
                        {
                            foreach (string nome in query)
                            {
                                if (node.ChildNodes[2].InnerText.Trim().ToLower().Contains(nome.ToLower().Trim()))
                                {
                                    listaRanking.Add(new RankingDto()
                                    {
                                        Posicao = posicao++.ToString(),
                                        RankingUri = node.ChildNodes[0].InnerText.Trim(),
                                        Nome = node.ChildNodes[2].InnerText.Trim(),
                                        Resolvidos = node.ChildNodes[3].InnerText.Trim(),
                                        Tentativas = node.ChildNodes[4].InnerText.Trim(),
                                        Submissoes = node.ChildNodes[5].InnerText.Trim(),
                                        Pontos = node.ChildNodes[6].InnerText.Trim()
                                    });

                                    break;
                                }
                            }
                        }
                    }
                }

                return listaRanking;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Trata a queryString recebida para pesquisar no crowler
        /// Quebra a queryString em uma lista
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private List<string> TratarQueryString(string queryString)
        {
            List<string> result = new List<string>();

            while (queryString.Contains("  "))
            {
                //replace 2 espaços por 1 espaço
                queryString = queryString.Replace("  ", " "); 
            }

            if (queryString.Contains(" "))
            {
                //split no char espaço
                result.AddRange(queryString.Split(" "));
            }
            else
            {
                result.Add(queryString);
            }

            return result;
        }

        /// <summary>
        /// Verifica o total de páginas no ranking da faculdade
        /// </summary>
        /// <returns></returns>
        private int GetTotalPaginas()
        {
            string paginaHtml = extratorHtml.CarregarPaginaHtml(linkUriRankingUnasp);

            var result = ExtrairNodeText(paginaHtml, @"//div[@id='table-info']").Split(" ");

            return int.Parse(result[2]);
        }

        /// <summary>
        /// Extrair o texto do node que der match com o pathExpression
        /// </summary>
        /// <param name="paginaHtml"></param>
        /// <param name="pathExpression"></param>
        /// <returns></returns>
        private string ExtrairNodeText(string paginaHtml, string pathExpression)
        {
            try
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(paginaHtml);

                return htmlDocument.DocumentNode.SelectSingleNode(pathExpression).InnerText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
