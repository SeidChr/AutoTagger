namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard.V1;

    using HtmlAgilityPack;

    using Newtonsoft.Json;

    public class ImageDetailPageCrawler : HttpCrawler
    {

        public string Parse(string url)
        {
            var document = this.FetchDocument(url);
            var scriptNode = GetScriptNodeData(document);
            var userName = GetImageNode(scriptNode);
            return userName;
        }

        private static dynamic GetImageNode(dynamic data)
        {
            if (data == null)
            {
                return null;
            }
            return data.entry_data?.PostPage?[0]?.graphql?.shortcode_media?.owner?.username.ToString();
        }
    }
}
