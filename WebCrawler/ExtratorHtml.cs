using System;
using System.IO;
using System.Net;
using System.Text;

namespace UriWebApi.WebCrawler
{
    public class ExtratorHtml
    {
        public string CarregarPaginaHtml(string url, string valorParametro = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(valorParametro))
                {
                    url = string.Format(url, valorParametro);
                }

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));

                string read = streamReader.ReadToEnd();

                streamReader.Close();
                httpWebResponse.Close();

                return TratarResponse(read);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string TratarResponse(string response)
        {
            return response.Replace("\n", "").Replace("\t", "");
        }
    }
}
