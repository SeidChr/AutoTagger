using System;
using System.Collections.Generic;
using System.Text;
using AutoTagger.Contract;

namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    class ExploreTagsCrawler : ImagesCrawler
    {
        private const int MinPostsForHashtags = 1 * 1000 * 1000;

        public ExploreTagsCrawler()
        {
            this.MinHashTagCount = 0;
            this.MinLikes        = 0;
        }

        public override IEnumerable<IImage> Parse(string url)
        {
            var data = this.GetData(url);
            if (!HashtagHasEnoughPosts(data))
            {
                yield break;
            }
            var nodes = GetNodes(data);
            var images = this.GetImages(nodes);

            foreach (IImage image in images)
            {
                yield return image;
            }
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

        private dynamic GetNodes(dynamic data)
        {
            return data?.entry_data?.TagPage?[0]?.graphql?.hashtag?.edge_hashtag_to_top_posts?.edges;
        }
    }
}
