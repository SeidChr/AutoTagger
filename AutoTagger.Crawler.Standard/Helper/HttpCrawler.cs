using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text.RegularExpressions;

    using HtmlAgilityPack;

    using Newtonsoft.Json;

    public abstract class HttpCrawler
    {
        private static readonly Regex FindJson = new Regex(
            @"\s*window\s*\.\s*_sharedData\s*\=\s*(.*)\s*\;\s*",
            RegexOptions.Compiled);

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

        protected static dynamic GetScriptNodeData(HtmlNode document)
        {
            var scriptNode = document?.SelectNodes("//script")
                ?.FirstOrDefault(n => n.InnerText.Contains("window._sharedData = "));

            if (scriptNode == null)
            {
                return null;
            }

            var match = FindJson.Match(scriptNode.InnerText);
            if (!match.Success || !match.Groups[1].Success)
            {
                return null;
            }

            var json = match.Groups[1].Value;
            return JsonConvert.DeserializeObject(json);
        }
    }
}
