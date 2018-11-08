using System;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using System.Net;
using System.Net.Http;
using AngleSharp.Extensions;
using System.Collections.Generic;

namespace WebApplication1
{
    class PopularParser
    {
        public static HttpClient client;
        public static string htmlPage { get; set; }
        public static List<string> titleList = new List<string>();

        async public Task<string> PopularParserInit()
        {
            client = new HttpClient();
            for (int i = 0; i < 5; i++)
            {
                var link = "https://amdm.ru/akkordi/popular/page" + i.ToString() + "/";
                htmlPage = await GetHtml(link);
                GetLinks();
            }
            return "ok";
        }

        public async Task<string> GetHtml(string url)
        {
            var response = await client.GetAsync(url);
            string sourse = null;

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                sourse = await response.Content.ReadAsStringAsync();
            }
            return sourse;
        }

        public static void GetLinks()
        {

            var parser = new HtmlParser();
            var document = parser.Parse(htmlPage);
            var pp = document.QuerySelectorAll(".artist_name");
            foreach (var p in pp)
            {
                titleList.Add(p.Text());
            }
        }

        public string Get()
        {
            var rand = new Random();
            int r = rand.Next(titleList.Count);
            while (titleList[rand.Next(titleList.Count)] == null || titleList[r].IndexOf("на сайте") > 0)
            {
                r = rand.Next(titleList.Count);
            }
            return titleList[r];
        }

    }
}
