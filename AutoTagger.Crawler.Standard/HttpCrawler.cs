using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using System.Net.Http;

    using HtmlAgilityPack;

    public abstract class HttpCrawler
    {
        protected readonly HttpClient HttpClient;

        protected HttpCrawler()
        {
            this.HttpClient = new HttpClient();
        }

        protected HtmlNode FetchDocument(string url)
        {
            HttpResponseMessage result;
            try
            {
                result = this.HttpClient.GetAsync(url).Result;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception while fetching url " + url);
                return null;
            }

            var document = new HtmlDocument();
            document.Load(result.Content.ReadAsStreamAsync().Result);
            return document.DocumentNode;
        }
    }
}
