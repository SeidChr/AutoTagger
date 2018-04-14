namespace AutoTagger.Crawler.Standard.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using AutoTagger.Contract;

    using HtmlAgilityPack;
    using Newtonsoft.Json;

    class ImagesOverviewPageCrawler : HttpCrawler
    {
        public enum PageType
        {
            None,
            ExploreTags,
            Profile
        }

        private const int MinimumHashTagCount = 5;
        private const int MinimumLikes = 100;
        private static readonly Regex FindHashTagsRegex = new Regex(@"#\w+", RegexOptions.Compiled);

        //public event Action<IImage> FoundImage;

        public IEnumerable<IImage> Parse(string url, PageType currentPageType)
        {
            var document = this.FetchDocument(url);
            var scriptNode = GetScriptNodeData(document);
            var nodes = GetImageNodes(scriptNode, currentPageType);
            return GetImages(nodes);
        }

        private static dynamic GetImageNodes(dynamic data, PageType currentPageType)
        {
            if (data == null)
            {
                return null;
            }

            dynamic nodes = null;
            switch (currentPageType)
            {
                case PageType.ExploreTags:
                    nodes = data?.entry_data?.TagPage?[0]?.graphql?.hashtag?.edge_hashtag_to_top_posts?.edges;
                    break;
                case PageType.Profile:
                    nodes = data?.entry_data?.ProfilePage?[0]?.graphql?.user?.edge_owner_to_timeline_media?.edges;
                    break;
            }

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

                var innerNode = node.node;
                var image = new Image
                {
                    Likes = likes,
                    CommentCount = innerNode.edge_media_to_comment.count,
                    ImageId = innerNode.shortcode,
                    HumanoidTags = hashTags,
                    ImageUrl = innerNode.display_url,
                    User = "",
                    Follower = 0,
                    InstaUrl = innerNode.thumbnail_src
                };
                yield return image;
                //this.OnFoundImage(image);

                //yield return node.node.shortcode;
            }
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
