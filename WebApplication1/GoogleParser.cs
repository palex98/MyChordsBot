using AngleSharp.Parser.Html;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication1
{
    class GoogleParser
    {
        string UserRequest { set; get; }
        HttpClient Client;
        string baseUrl = "https://www.google.com/search?q=";
        string LinkToSong { set; get; }

        public GoogleParser(string UserRequest)
        {
            this.UserRequest = UserRequest;
            //await GetLinkToAmDm();
        }

        public async Task<string> GetLinkToAmDm()
        {
            var googleUrl = baseUrl + UserRequest;
            var googleHtmlPage = await GetHtml(googleUrl);
            return LinkToSong = GetLinkFromGoogle(googleHtmlPage);
        }

        async Task<string> GetHtml(string url)
        {
            Client = new HttpClient();
            var response = await Client.GetAsync(url);
            string sourse = null;

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                sourse = await response.Content.ReadAsStringAsync();
            }
            return sourse;
        }

        string GetLinkFromGoogle(string googleHtmlPage)
        {

            var parser = new HtmlParser();
            var document = parser.Parse(googleHtmlPage);
            var pp = document.GetElementsByTagName("a");
            for (int i = 0; i < pp.Length; i++)
            {
                if (pp[i].GetAttribute("href").IndexOf("https://amdm.ru/akkordi/") > 2)
                {
                    return "https://www.google.com" + pp[i].GetAttribute("href");
                }
            }
            return "";
        }
    }
}
