namespace AutoTagger.ImageProcessor.Standard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoTagger.Contract;
    using AutoTagger.Crawler.Standard;

    using global::Clarifai.API;
    using global::Clarifai.DTOs.Inputs;

    public class ClarifaiImageTagger : ITaggingProvider
    {
        private readonly ClarifaiClient client;

        public ClarifaiImageTagger()
        {
            var clarifaiApiKey = Environment.GetEnvironmentVariable("instatagger_clarifai_key");
            this.client = new ClarifaiClient(clarifaiApiKey);
        }

        public IEnumerable<IMTag> GetTagsForImageBytes(byte[] bytes)
        {
            var clarifaiInput = new ClarifaiFileImage(bytes);
            return this.GetTagsForInput(clarifaiInput);
        }

        public IEnumerable<IMTag> GetTagsForImageUrl(string imageUrl)
        {
            var clarifaiInput = new ClarifaiURLImage(imageUrl);
            return this.GetTagsForInput(clarifaiInput);
        }

        private IEnumerable<IMTag> GetTagsForInput(IClarifaiInput clarifaiInput)
        {
            var result = this.client.PublicModels.GeneralModel.Predict(clarifaiInput).ExecuteAsync().Result;
            return !result.IsSuccessful
                ? Enumerable.Empty<IMTag>()
                : result.Get().Data.Select(x => new MTag {Name = x.Name});
        }
    }
}
