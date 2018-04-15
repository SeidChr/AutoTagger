namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoTagger.Contract;

    class UserCrawler : ImagesCrawler
    {
        private const int MinFollowerCount = 1000;

        public UserCrawler()
        {
            this.MinHashTagCount = 5;
            this.MinLikes        = 300;
        }

        public override IEnumerable<IImage> Parse(string url)
        {
            var data = this.GetData(url);

            if (!HasUserEnoughFollower(data, out int followerCount))
            {
                yield break;
            }

            var nodes  = GetNodes(data);
            IEnumerable<IImage> images = this.GetImages(nodes);
            var imagesList = images.ToList();

            if (AreAllHashtagsTheSame(imagesList))
            {
                var count = imagesList.Count;
                var index = new Random().Next(0, count);
                imagesList[index].Follower = followerCount;
                yield return imagesList[index];
                yield break;
            }

            foreach (IImage image in imagesList)
            {
                image.Follower = followerCount;
                yield return image;
            }
        }

        private static bool AreAllHashtagsTheSame(IEnumerable<IImage> images)
        {
            if (!images.Any())
                return false;
            var previousHashTags = "";
            foreach (IImage image in images)
            {
                var hashTags = string.Join("", image.HumanoidTags);
                if (previousHashTags != "" && hashTags != previousHashTags)
                    return false;
                previousHashTags = hashTags;
            }
            return true;
        }

        private static bool HasUserEnoughFollower(dynamic data, out int followerCount)
        {
            var node      = data?.entry_data?.ProfilePage?[0]?.graphql?.user?.edge_followed_by;
            followerCount = Convert.ToInt32(node?.count.ToString());
            return followerCount >= MinFollowerCount;
        }

        private dynamic GetNodes(dynamic data)
        {
            return data?.entry_data?.ProfilePage?[0]?.graphql?.user?.edge_owner_to_timeline_media?.edges;
        }
    }
}
