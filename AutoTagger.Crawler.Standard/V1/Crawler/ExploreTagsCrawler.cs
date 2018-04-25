namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;

    class ExploreTagsCrawler : ImageCrawler
    {
        private const int MinPostsForHashtags = 1 * 1000 * 1000;
        //public Dictionary<string, int> Data;

        public ExploreTagsCrawler()
        {
            this.MinHashTagCount = 0;
            this.MinLikes        = 100;
            //this.Data = new Dictionary<string, int>();
        }

        public (int, List<IImage>) Parse(string url)
        {
            var data = this.GetData(url);
            if (!HashtagHasEnoughPosts(data))
            {
                //yield break;
                return (0, null);
            }

            var amountPosts = GetAmountOfPosts(data);
            var nodes  = GetTopPostsNodes(data);
            IEnumerable<IImage> images = this.GetImages(nodes);
            var imagesList = images.ToList();

            return (amountPosts, imagesList);
            //foreach (IImage image in images)
            //{
            //    yield return image;
            //}
        }

        private int GetAmountOfPosts(dynamic data)
        {
            var hashtagNodes  = GetHashtagNodes(data);
            //var name = hashtagNodes?.name.ToString();
            var amountPosts = Convert.ToInt32(hashtagNodes?.edge_hashtag_to_media?.count.ToString());
            //this.Data.Add(name, amountPosts);
            return amountPosts;
        }

        private static bool HashtagHasEnoughPosts(dynamic data)
        {
            if (data == null)
            {
                return false;
            }

            dynamic node = data.entry_data?.TagPage?[0]?.graphql?.hashtag?.edge_hashtag_to_media;
            var amountOfPosts = Convert.ToInt32(node?.count.ToString());
            return amountOfPosts >= MinPostsForHashtags;
        }

        private dynamic GetTopPostsNodes(dynamic data)
        {
            return data?.entry_data?.TagPage?[0]?.graphql?.hashtag?.edge_hashtag_to_top_posts?.edges;
        }

        private dynamic GetHashtagNodes(dynamic data)
        {
            return data?.entry_data?.TagPage?[0]?.graphql?.hashtag;
        }
    }
}
