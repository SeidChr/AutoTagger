namespace AutoTagger.Crawler.Standard.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using AutoTagger.Contract;

    abstract class ImageCrawler : HttpCrawler
    {
        protected static int MaxHashtagLength = 30;
        protected static int MinHashtagLength = 5;
        private static readonly Regex FindHashTagsRegex = new Regex(@"#\w+", RegexOptions.Compiled);
        protected int MinCommentsCount = 0;
        protected int MinHashTagCount = 0;
        protected int MinLikes = 0;

        public static DateTime GetDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        protected dynamic GetData(string url)
        {
            var document = this.FetchDocument(url);
            return GetScriptNodeData(document);
        }

        protected IEnumerable<IImage> GetImages(dynamic nodes)
        {
            if (nodes == null)
            {
                yield break;
            }

            foreach (var node in nodes)
            {
                var edges = node?.node?.edge_media_to_caption?.edges;
                if (edges == null)
                {
                    continue;
                }

                if (edges.ToString() == "[]")
                {
                    continue;
                }

                string text  = edges[0]?.node?.text;
                text         = text?.Replace("\\n", "\n");
                text         = System.Web.HttpUtility.HtmlDecode(text);
                var hashTags = ParseHashTags(text).ToList();

                var innerNode     = node.node;
                int likes         = innerNode.edge_liked_by?.count;
                var hashTagsCount = hashTags.Count;
                var commentsCount = innerNode?.edge_media_to_comment?.count;

                if (hashTagsCount < this.MinHashTagCount || likes < this.MinLikes
                 || commentsCount < this.MinCommentsCount)
                {
                    continue;
                }

                var takenDate = GetDateTime(Convert.ToDouble(innerNode?.taken_at_timestamp.ToString()));
                var image     = new Image
                {
                    Likes        = likes,
                    Comments     = commentsCount,
                    Shortcode    = innerNode?.shortcode,
                    HumanoidTags = hashTags,
                    LargeUrl     = innerNode?.display_url,
                    ThumbUrl     = innerNode?.thumbnail_src,
                    Uploaded     = takenDate
                };
                yield return image;
            }
        }

        private static bool HashtagIsAllowed(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.Length >= MinHashtagLength
                && value.Length < MaxHashtagLength
                && !IsDigitsOnly(value);
        }

        static bool IsDigitsOnly(string str)
        {
            foreach (var c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private static IEnumerable<string> ParseHashTags(string text)
        {
            if (text == null)
            {
                return Enumerable.Empty<string>();
            }

            return FindHashTagsRegex.Matches(text).OfType<Match>().Select(m => m?.Value.Trim(' ', '#').ToLower())
                .Where(HashtagIsAllowed).Distinct();
        }
    }
}
