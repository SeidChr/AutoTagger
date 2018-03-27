namespace AutoTagger.Crawler.Standard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using AutoTagger.Contract;

    using Newtonsoft.Json;

    public class InstagramCrawlerV2 : HttpCrawler
    {
        private static readonly Regex FindHashTagsRegex = new Regex(@"#\w+", RegexOptions.Compiled);

        private static readonly Regex FindJsonRegex = new Regex(
            @"\s*window\s*\.\s*_sharedData\s*\=\s*(.*)\s*\;\s*",
            RegexOptions.Compiled);

        public IEnumerable<string> Get(string url)
        {
            var document = this.FetchDocument(url);
            var scriptNode = document.SelectNodes("//script")
                .FirstOrDefault(n => n.InnerText.Contains("window._sharedData = "));
            if (scriptNode == null)
            {
                yield break;
            }

            var match = FindJsonRegex.Match(scriptNode.InnerText);
            if (!match.Success || !match.Groups[1].Success)
            {
                yield break;
            }

            dynamic instaDataArray = JsonConvert.DeserializeObject(match.Groups[1].Value);

            ////entry_data.TagPage[0].graphql.hashtag.edge_hashtag_to_top_posts.edges[*].node
            ////    .shortcode
            ////    .edge_media_to_caption.edges[0].node.text
            ////    .edge_media_to_comment.count
            ////    .edge_liked_by.count
            ////    .display_url
            var nodes = instaDataArray.entry_data.TagPage[0].graphql.hashtag.edge_hashtag_to_top_posts.edges;
            foreach (var x in nodes)
            {
                string imageText = x.node.edge_media_to_caption.edges[0].node.text;
                imageText = imageText?.Replace("\\n", "\n");
                imageText = System.Web.HttpUtility.HtmlDecode(imageText);
                var hashTags = this.ParseHashTags(imageText);

                yield return x.node.shortcode + "(" + x.node.edge_liked_by.count + ", "
                           + x.node.edge_media_to_comment.count + ", [" + string.Join(", ", hashTags) + "])";
            }
        }

        public ICrawlerImage GetCrawlerImageForImageId(string imageId)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<string> ParseHashTags(string text)
        {
            if (text == null)
            {
                return Enumerable.Empty<string>();
            }

            return FindHashTagsRegex.Matches(text).OfType<Match>().Select(m => m?.Value.Trim(' ', '#'))
                .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();
        }
    }
}
