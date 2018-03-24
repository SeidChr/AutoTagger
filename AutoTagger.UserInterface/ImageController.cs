using System;
using System.Collections.Generic;
using System.Linq;
using AutoTagger.Clarifai.Standard;
using AutoTagger.Contract;
using AutoTagger.Database.Standard;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AutoTagger.UserInterface
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {

        // POST api/<controller>
        [HttpPost]
        public JsonResult Post([FromForm]string link)
        {
            var content = new Dictionary<string, object>();
            content.Add("link", link);

            var _taggingProvider = new ClarifaiImageTagger() as ITaggingProvider;
            var _db = new AutoTaggerDatabase() as IAutoTaggerDatabase;

            var maschineTags = _taggingProvider.GetTagsForImage(link).ToList();
            content.Add("maschineTags", maschineTags);

            try
            {
                var instagramTags = _db.FindInstagramTags(maschineTags);
                content.Add("instagramTags", instagramTags);
            }
            catch (NotImplementedException)
            {

            }

            return Json(content);
        }
    }
}
