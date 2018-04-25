namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoTagger.Contract;

    class UserCrawler : ImageCrawler
    {
        private const int MinFollowerCount = 1000;

        public UserCrawler()
        {
            this.MinHashTagCount  = 5;
            this.MinCommentsCount = 10;
            this.MinLikes         = 300;
        }

        public IEnumerable<IImage> Parse(string url)
        {
            var data = this.GetData(url);

            if (!HasUserEnoughFollower(data, out int followerCount, out int followingCount, out int postsCount))
            {
                yield break;
            }

            var nodes = GetNodes(data);
            IEnumerable<IImage> images = this.GetImages(nodes);
            var imagesList = images.ToList();

            foreach (IImage image in imagesList)
            {
                image.Follower  = followerCount;
                image.Following = followingCount;
                image.Posts = postsCount;
            }

            images = RemoveImagesWithDuplicateHashtags(imagesList);

            foreach (IImage image in images)
            {
                yield return image;
            }
        }

        private IEnumerable<IImage> RemoveImagesWithDuplicateHashtags(List<IImage> images)
        {
            var newImages = new Dictionary<string, IImage>();
            for (int i = images.Count-1; i >= 0; i--)
            {
                var image = images[i];
                var hashTags = string.Join("", image.HumanoidTags);
                if (!newImages.ContainsKey(hashTags))
                    newImages.Add(hashTags, image);
            }
            foreach (var newImage in newImages)
            {
                yield return newImage.Value;
            }
        }

        private static bool HasUserEnoughFollower(dynamic data, out int followerCount, out int followingCount, out int postsCount)
        {
            var node      = data?.entry_data?.ProfilePage?[0]?.graphql?.user;
            followerCount = Convert.ToInt32(node?.edge_followed_by?.count.ToString());
            followingCount = Convert.ToInt32(node?.edge_follow?.count.ToString());
            postsCount = Convert.ToInt32(node?.edge_owner_to_timeline_media?.count.ToString());
            return followerCount >= MinFollowerCount;
        }

        private dynamic GetNodes(dynamic data)
        {
            return data?.entry_data?.ProfilePage?[0]?.graphql?.user?.edge_owner_to_timeline_media?.edges;
        }
    }
}
