using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Clarifai.Standard
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoTagger.Contract;

    using Google.Cloud.Vision.V1;

    public class GCPVision : ITaggingProvider
    {
        private readonly ImageAnnotatorClient client;

        public GCPVision()
        {
            client = ImageAnnotatorClient.Create();
        }

        public IEnumerable<string> GetTagsForImageBytes(byte[] imageBytes)
        {
            //var image = Image.FromFile("wakeupcat.jpg");
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTagsForImageUrl(string imageUrl)
        {
            var image = Image.FromUri(imageUrl);

            var labels = client.DetectLabels(image);
            var webInfos = client.DetectWebInformation(image);

            foreach (var annotation in labels)
            {
                if (annotation.Description == null)
                    continue;

                Console.WriteLine(annotation.Description);
                yield return annotation.Description;
            }
        }
    }
}
