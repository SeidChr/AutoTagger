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
        private const int MinimumHashTagCount = 5;

        private static readonly Regex FindHashTagsRegex = new Regex(@"#\w+", RegexOptions.Compiled);

        private static readonly Regex FindJsonRegex = new Regex(
            @"\s*window\s*\.\s*_sharedData\s*\=\s*(.*)\s*\;\s*",
            RegexOptions.Compiled);

        public event Action<ICrawlerImage> FoundImage;

        public ICrawlerImage GetCrawlerImageForImageId(string imageId)
        {
            throw new NotImplementedException();
        }

        public void ParseHashTagPage(string hashtag)
        {
            var document = this.FetchDocument($"https://www.instagram.com/explore/tags/{hashtag}/");

            var scriptNode = document?.SelectNodes("//script")
                .FirstOrDefault(n => n.InnerText.Contains("window._sharedData = "));

            if (scriptNode == null)
            {
                ////yield break;
                return;
            }

            var match = FindJsonRegex.Match(scriptNode.InnerText);
            if (!match.Success || !match.Groups[1].Success)
            {
                ////yield break;
                return;
            }

            dynamic instaDataArray = JsonConvert.DeserializeObject(match.Groups[1].Value);

            ////entry_data.TagPage[0].graphql.hashtag.edge_hashtag_to_top_posts.edges[*].node
            ////    .shortcode
            ////    .edge_media_to_caption.edges[0].node.text
            ////    .edge_media_to_comment.count
            ////    .edge_liked_by.count
            ////    .display_url
            var nodes = instaDataArray?.entry_data?.TagPage[0]?.graphql?.hashtag?.edge_hashtag_to_top_posts?.edges;
            if (nodes == null)
            {
                return;
            }

            foreach (var x in nodes)
            {
                string imageText = x.node.edge_media_to_caption.edges[0].node.text;
                imageText = imageText?.Replace("\\n", "\n");
                imageText = System.Web.HttpUtility.HtmlDecode(imageText);
                var hashTags = this.ParseHashTags(imageText).ToList();
                if (hashTags.Count < MinimumHashTagCount)
                {
                    return;
                }

                this.OnFoundImage(
                    new CrawlerImage
                    {
                        Likes        = x.node.edge_liked_by.count,
                        Comments     = x.node.edge_media_to_comment.count,
                        ImageId      = x.node.shortcode,
                        HumanoidTags = hashTags,
                        ImageUrl     = x.node.display_url
                    });
            }
        }

        protected virtual void OnFoundImage(ICrawlerImage image)
        {
            this.FoundImage?.Invoke(image);
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
