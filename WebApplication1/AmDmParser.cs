using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication1
{
    class AmDmParser
    {
        string LinkToSong { set; get; }
        string AmDmHtmlPage { set; get; }
        HttpClient Client;

        public AmDmParser(string LinkToSong)
        {
            this.LinkToSong = LinkToSong;
        }

        public async Task<string> GetData()
        {
            AmDmHtmlPage = await GetHtml();
            string data = "*" + GetTitle() + "*" + "\n\n" + GetTextOfSong();
            Console.WriteLine(data.Length);
            if (data.Length > 4076)
            {
                data = data.Substring(0, 4056);
            }
            return data + "\n\n" + "*🔵 Тональность: 0*";
        }

        async Task<string> GetHtml()
        {
            Client = new HttpClient();
            var response = await Client.GetAsync(LinkToSong);
            string sourse = null;

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                sourse = await response.Content.ReadAsStringAsync();
            }
            return sourse;
        }
        public string GetTextOfSong()
        {

            var parser = new HtmlParser();
            var document = parser.Parse(AmDmHtmlPage);

            var pp = document.QuerySelectorAll("div.b-podbor__text");

            return pp[0].Text().Replace("*", "");
        }

        public string GetTitle()
        {

            var parser = new HtmlParser();
            var document = parser.Parse(AmDmHtmlPage);

            var pp = document.QuerySelectorAll("div.b-podbor");

            var title = pp[0].Text().Substring(0, pp[0].Text().IndexOf(", аккор"));

            return title.Replace("*", "");
        }
    }
}
