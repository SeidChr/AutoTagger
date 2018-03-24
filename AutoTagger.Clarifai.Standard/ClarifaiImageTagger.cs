using System.Collections.Generic;
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

        public IEnumerable<string> GetTagsForImage(string imageUrl)
        {
            //return null;
            var clarifaiUrlImage = new ClarifaiURLImage(imageUrl);
            var result = client.PublicModels.GeneralModel.Predict(clarifaiUrlImage).ExecuteAsync().Result;
            return !result.IsSuccessful 
                ? Enumerable.Empty<string>() 
                : result.Get().Data.Select(x => x.Name);
        }
    }
}
