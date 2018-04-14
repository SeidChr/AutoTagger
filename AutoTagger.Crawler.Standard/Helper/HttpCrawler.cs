using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using System.Net;
    using System.Net.Http;

    using HtmlAgilityPack;

    public abstract class HttpCrawler
    {
        private static HttpClient httpClient;

        protected static HttpClient HttpClient
        {
            get
            {
                if (httpClient == null)
                {
                    httpClient = new HttpClient();
                }
                return httpClient;
            }
        }

        protected HtmlNode FetchDocument(string url)
        {
            HttpResponseMessage result;
            try
            {
                result = HttpClient.GetAsync(url).Result;
                var status = result.StatusCode;
                if (status != HttpStatusCode.OK)
                {
                    return null;
                }
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
