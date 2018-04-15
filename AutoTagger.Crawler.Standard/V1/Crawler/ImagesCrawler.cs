namespace AutoTagger.Crawler.Standard.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using AutoTagger.Contract;

    abstract class ImagesCrawler : HttpCrawler
    {
        protected int MinHashTagCount = 0;
        protected int MinLikes = 0;
        protected static int MinHashtagLength = 5;
        private static readonly Regex FindHashTagsRegex = new Regex(@"#\w+", RegexOptions.Compiled);

        public abstract IEnumerable<IImage> Parse(string url);

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
                string text = edges[0]?.node?.text;
                text = text?.Replace("\\n", "\n");
                text = System.Web.HttpUtility.HtmlDecode(text);
                var hashTags = ParseHashTags(text).ToList();

                int likes = node.node?.edge_liked_by?.count;
                if (!this.MeetsConditions(hashTags.Count, likes))
                {
                    continue;
                }

                var innerNode = node.node;
                var takenDate = GetDateTime(Convert.ToDouble(innerNode?.taken_at_timestamp.ToString()));
                var image = new Image
                {
                    Likes = likes,
                    CommentCount = innerNode?.edge_media_to_comment?.count,
                    Shortcode = innerNode?.shortcode,
                    HumanoidTags = hashTags,
                    Url = innerNode?.display_url,
                    Uploaded = takenDate
                };
                yield return image;
            }
        }

        private bool MeetsConditions(int hashTagsCount, int likes)
        {
            return hashTagsCount >= MinHashTagCount && likes >= MinLikes;
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

        private static bool HashtagIsAllowed(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length >= MinHashtagLength && !IsDigitsOnly(value);
        }

        public static DateTime GetDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
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
    }
}
