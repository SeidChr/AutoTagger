using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoTagger.Clarifai.Standard;
using Nancy;
using Newtonsoft.Json;

namespace AutoTagger.TaggingProvider
{
    public class TagMobule : NancyModule
    {
        public TagMobule()
        {
            var imageTagger = new ClarifaiImageTagger();

            //Get["/find/{link}"] = parameters =>
            //{
            //    var link = this.Request.Query["link"];
            //    var tags =  imageTagger.GetTagsForImage(link);
            //    return JsonConvert.SerializeObject(tags);
            //};

        }
    }
}
