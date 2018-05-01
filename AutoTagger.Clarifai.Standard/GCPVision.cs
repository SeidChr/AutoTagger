namespace AutoTagger.ImageProcessor.Standard
{
    using System.Collections.Generic;
    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;
    using Google.Cloud.Vision.V1;
    using Image = Google.Cloud.Vision.V1.Image;

    public class GCPVision : ITaggingProvider
    {
        private const string keyLabel = "GCPVision_Label";
        private const string keyWeb = "GCPVision_Web";
        private readonly ImageAnnotatorClient client;

        public GCPVision()
        {
            this.client = ImageAnnotatorClient.Create();
        }

        public IEnumerable<IMTag> GetTagsForImageBytes(byte[] bytes)
        {
            var image = Image.FromBytes(bytes);

            var labels   = this.client.DetectLabels(image);
            var webInfos = this.client.DetectWebInformation(image);

            foreach (var mTag in ToMTags(labels, webInfos))
            {
                yield return mTag;
            }
        }

        public IEnumerable<IMTag> GetTagsForImageUrl(string imageUrl)
        {
            var image = Image.FromUri(imageUrl);

            var labels   = this.client.DetectLabels(image);
            var webInfos = this.client.DetectWebInformation(image);

            foreach (var mTag in ToMTags(labels, webInfos))
            {
                yield return mTag;
            }
        }

        private static IEnumerable<IMTag> ToMTags(IReadOnlyList<EntityAnnotation> labels, WebDetection webInfos)
        {
            foreach (var x in labels)
            {
                if (x.Description == null)
                    continue;
                var mtag = new MTag { Name = x.Description, Score = x.Score, Source = keyLabel };
                yield return mtag;
            }

            foreach (var x in webInfos.WebEntities)
            {
                if (x.Description == null)
                    continue;
                var mtag = new MTag { Name = x.Description, Score = x.Score, Source = keyWeb };
                yield return mtag;
            }
        }
    }
}
