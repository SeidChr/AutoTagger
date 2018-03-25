using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoTagger.Contract;
using Clarifai.API;
using Clarifai.DTOs.Inputs;

namespace AutoTagger.Clarifai.Standard
{
    public class ClarifaiImageTagger : ITaggingProvider
    {
        private const string ClarifaiApiKey = "c8747218d8bc496a8de7e761d8f593e6";

        private readonly ClarifaiClient client;

        public ClarifaiImageTagger()
        {
            client = new ClarifaiClient(ClarifaiApiKey);
        }

        public IEnumerable<string> GetTagsForImageUrl(string imageUrl)
        {
            var clarifaiInput = new ClarifaiURLImage(imageUrl);
            return GetTagsForInput(clarifaiInput);
        }

        public IEnumerable<string> GetTagsForImageBytes(byte[] imageBytes)
        {
            var clarifaiInput = new ClarifaiFileImage(imageBytes);
            return GetTagsForInput(clarifaiInput);
        }

        private IEnumerable<string> GetTagsForInput(IClarifaiInput clarifaiInput)
        {
            var result = client.PublicModels.GeneralModel.Predict(clarifaiInput).ExecuteAsync().Result;
            return !result.IsSuccessful
                ? Enumerable.Empty<string>()
                : result.Get().Data.Select(x => x.Name);
        }
    }
}
