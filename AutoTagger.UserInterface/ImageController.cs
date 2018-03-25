using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoTagger.Clarifai.Standard;
using AutoTagger.Contract;
using AutoTagger.Database.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AutoTagger.UserInterface
{
    [Route("[controller]")]
    public class ImageController : Controller
    {
        private readonly IAutoTaggerDatabase _db;
        private readonly ITaggingProvider _taggingProvider;

        public ImageController(IAutoTaggerDatabase db, ITaggingProvider taggingProvider)
        {
            _db = db;
            _taggingProvider = taggingProvider;
        }

        // POST api/<controller>
        [HttpPost]
        public JsonResult Post([FromForm]string link)
        {
            return DoTagging(link);
        }

        private JsonResult DoTagging(string link)
        {
            var content = new Dictionary<string, object>();
            content.Add("link", link);

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

        // POST api/<controller>/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var content = "";

            if (!Request.ContentType.Contains("multipart/form-data; boundary"))
            {
                return BadRequest("wrong contentType :'(");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No Files uploaded");
            }


            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("Upload successful");
        }

    }
}
