using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    using AutoTagger.Contract;

    class UserCrawler : ImagesCrawler
    {
        private const int MinFollowerCount = 1000;

        public UserCrawler()
        {
            this.MinHashTagCount = 10;
            this.MinLikes = 300;
        }

        public override IEnumerable<IImage> Parse(string url)
        {
            var data = this.GetData(url);

            if (!HasUserEnoughFollower(data, out int followerCount))
            {
                yield break;
            }

            var nodes  = GetNodes(data);
            var images = this.GetImages(nodes);

            foreach (IImage image in images)
            {
                image.Follower = followerCount;
                yield return image;
            }
        }

        private static bool HasUserEnoughFollower(dynamic data, out int followerCount)
        {
            var node = data?.entry_data?.ProfilePage?[0]?.graphql?.user?.edge_followed_by;
            followerCount = Convert.ToInt32(node?.count.ToString());
            return followerCount >= MinFollowerCount;
        }

        private dynamic GetNodes(dynamic data)
        {
            return data?.entry_data?.ProfilePage?[0]?.graphql?.user?.edge_owner_to_timeline_media?.edges;
        }
    }
}
