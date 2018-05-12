namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;

    internal class ExploreTagsCrawler : ImageCrawler
    {
        private const int MinPostsForHashtags = 1 * 1000 * 1000;
        
        public ExploreTagsCrawler()
        {
            this.MinHashTagCount = 0;
            this.MinLikes        = 100;
        }

        public (int, List<IImage>) Parse(string url)
        {
            var data        = this.GetData(url);
            var amountPosts = GetAmountOfPosts(data);
            if (amountPosts < MinPostsForHashtags)
            {
                return (amountPosts, null);
            }

            var nodes      = GetTopPostsNodes(data);
            var imagesList = this.GetImages(nodes).ToList();

            return (amountPosts, imagesList);
        }

        private static int GetAmountOfPosts(dynamic data)
        {
            var hashtagNodes = GetHashtagNodes(data);
            var amountPosts  = Convert.ToInt32(hashtagNodes?.edge_hashtag_to_media?.count.ToString());
            return amountPosts;
        }

        private static dynamic GetHashtagNodes(dynamic data)
        {
            return data?.entry_data?.TagPage?[0]?.graphql?.hashtag;
        }

        private static dynamic GetTopPostsNodes(dynamic data)
        {
            return data?.entry_data?.TagPage?[0]?.graphql?.hashtag?.edge_hashtag_to_top_posts?.edges;
        }
    }
}
