namespace AutoTagger.Crawler.Standard.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using AutoTagger.Contract;

    using HtmlAgilityPack;
    using Newtonsoft.Json;

    class ExploreTagsPageCrawler : HttpCrawler
    {
        private const int MinimumHashTagCount = 5;

        private const int MinimumLikes = 100;

        private static readonly Regex FindHashTagsRegex = new Regex(@"#\w+", RegexOptions.Compiled);

        private static readonly Regex FindJsonRegex = new Regex(
            @"\s*window\s*\.\s*_sharedData\s*\=\s*(.*)\s*\;\s*",
            RegexOptions.Compiled);

        //public event Action<IImage> FoundImage;

        public IEnumerable<IImage> Parse(string url)
        {

            var document = this.FetchDocument(url);

            var scriptNode = GetScriptNode(document);

            var nodes = GetImageNodes(scriptNode);

            return GetImages(nodes);
        }

        private static dynamic GetImageNodes(HtmlNode scriptNode)
        {
            if (scriptNode == null)
            {
                return null;
            }

            var match = FindJsonRegex.Match(scriptNode.InnerText);
            if (!match.Success || !match.Groups[1].Success)
            {
                return null;
            }

            dynamic instaDataArray = JsonConvert.DeserializeObject(match.Groups[1].Value);

            var nodes = instaDataArray?.entry_data?.TagPage[0]?.graphql?.hashtag?.edge_hashtag_to_top_posts?.edges;
            if (nodes == null)
            {
                return null;
            }

            return nodes;
        }

        private static IEnumerable<IImage> GetImages(dynamic nodes)
        {
            if (nodes == null)
            {
                yield break;
            }

            foreach (var node in nodes)
            {
                string text = node.node.edge_media_to_caption.edges[0].node.text;
                text = text?.Replace("\\n", "\n");
                text = System.Web.HttpUtility.HtmlDecode(text);
                var hashTags = ParseHashTags(text).ToList();

                int likes = node.node.edge_liked_by.count;
                if (!MeetsConditions(hashTags.Count, likes))
                {
                    yield break;
                }

                var image = new Image
                {
                    Likes = likes,
                    CommentCount = node.node.edge_media_to_comment.count,
                    ImageId = node.node.shortcode,
                    HumanoidTags = hashTags,
                    ImageUrl = node.node.display_url
                };
                yield return image;
                //this.OnFoundImage(image);

                //yield return node.node.shortcode;
            }
        }

        private static HtmlNode GetScriptNode(HtmlNode document)
        {
            var scriptNode = document?.SelectNodes("//script")
                .FirstOrDefault(n => n.InnerText.Contains("window._sharedData = "));

            return scriptNode;
        }

        private static bool MeetsConditions(int hashTagsCount, int likes)
        {
            return hashTagsCount > MinimumHashTagCount && likes > MinimumLikes;
        }

        //protected virtual void OnFoundImage(IImage image)
        //{
        //    this.FoundImage?.Invoke(image);
        //}

        private static IEnumerable<string> ParseHashTags(string text)
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
