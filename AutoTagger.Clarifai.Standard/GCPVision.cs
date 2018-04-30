using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Clarifai.Standard
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;

    using Google.Cloud.Vision.V1;

    using Image = Google.Cloud.Vision.V1.Image;

    public class GCPVision : ITaggingProvider
    {
        private readonly ImageAnnotatorClient client;

        public GCPVision()
        {
            client = ImageAnnotatorClient.Create();
        }

        public IEnumerable<IMTag> GetTagsForImageBytes(byte[] imageBytes)
        {
            //var image = Image.FromFile("wakeupcat.jpg");
            throw new NotImplementedException();
        }

        public IEnumerable<IMTag> GetTagsForImageUrl(string imageUrl)
        {
            var image = Image.FromUri(imageUrl);

            var labels = client.DetectLabels(image);
            var webInfos = client.DetectWebInformation(image);
            
            foreach (var x in labels)
            {
                if (x.Description == null)
                    continue;
                var mtag = new MTag { Name = x.Description,
                    Score = x.Score,
                    Source = "GCPVision_Label" };
                yield return mtag;
            }

            foreach (var x in webInfos.WebEntities)
            {
                if (x.Description == null)
                    continue;
                var mtag = new MTag
                {
                    Name   = x.Description,
                    Score  = x.Score,
                    Source = "GCPVision_Web"
                };
                yield return mtag;
            }
        }
    }
}
