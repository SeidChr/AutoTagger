//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AutoTagger.Crawler.Standard.V1.Crawler
//{
//    using System.Linq;
//    using System.Text.RegularExpressions;

//    using AutoTagger.Contract;

//    public class UserPageCrawler : HttpCrawler
//    {
//        public IEnumerable<IImage> Parse(string userName)
//        {
//            var url = $"https://www.instagram.com/{userName}/?hl=en";

//            var document = this.FetchDocument(url);

//            var imageUrl = document.SelectNodes("//meta[@property='og:image']")?.FirstOrDefault()?.Attributes["content"]
//                ?.Value;

//            var qualityString = document.SelectNodes("//meta[@property='og:description']")?.FirstOrDefault()
//                ?.Attributes["content"]?.Value;

//            (int likes, int comments) = this.ExtractQualityFromDescription(qualityString);

//            var hashTags = document
//                .SelectNodes("//meta[@property='instExtractQualityFromDescriptionStringpp:hashtags']")
//                ?.Select(x => x?.Attributes["content"]?.Value).Where(tag => tag != null);

//            var shortcode = "";
//            var instaUrl  = "";
//            var follower  = 123;

//            // <meta property="og:description" content="132 Likes, 1 CommentCount - ..........">
//            var result = new Image
//            {
//                HumanoidTags = hashTags ?? Enumerable.Empty<string>(),
//                ImageId      = shortcode,
//                ImageUrl     = imageUrl,
//                InstaUrl     = instaUrl,
//                Likes        = likes,
//                CommentCount = comments,
//                Follower     = follower,
//                User         = userName
//            };

//            yield return result;
//        }

//        private (int likes, int comments) ExtractQualityFromDescription(string qualityString)
//        {
//            var likes    = 0;
//            var comments = 0;

//            if (string.IsNullOrWhiteSpace(qualityString))
//            {
//                return (likes, comments);
//            }

//            var qualityMatch = Regex.Match(qualityString, @"(\d+)\sLikes,\s(\d+)\sComments\s-\s");
//            if (!qualityMatch.Success)
//            {
//                return (likes, comments);
//            }

//            var likesString    = qualityMatch.Groups[1].Value;
//            var commentsString = qualityMatch.Groups[2].Value;

//            int.TryParse(likesString, out likes);
//            int.TryParse(commentsString, out comments);

//            return (likes, comments);
//        }
//    }
//}
