using System.Collections.Generic;
using AutoTagger.Contract;
using Clarifai.API;
using Clarifai.DTOs.Inputs;

namespace AutoTagger.Clarifai.Standard
{
    public class ClarifaiImageTagger : ITaggingProvider
    {
        private const string ClarifaiApiKey = "";

        private readonly ClarifaiClient client;

        public ClarifaiImageTagger()
        {
            client = new ClarifaiClient(ClarifaiApiKey);
        }

        public IEnumerable<string> GetTagsForImage(string imageUrl)
        {
            return null;
            var clarifaiUrlImage = new ClarifaiURLImage(imageUrl);
            var result = client.PublicModels.GeneralModel.Predict(clarifaiUrlImage).ExecuteAsync().Result;
            if (!result.IsSuccessful)
            {
                //return Enumerable<string>.Empty;
            }
        }
    }
}
