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
        public IActionResult Post([FromForm]string link)
        {
            var content = new Dictionary<string, object>();
            content.Add("link", link);

            var machineTags = _taggingProvider.GetTagsForImageUrl(link).ToList();
            content.Add("machineTags", machineTags);

            if (!machineTags.Any())
            {
                return BadRequest("No MachineTags found :'(");
            }

            var instagramTags = _db.FindHumanoidTags(machineTags);
            content.Add("instagramTags", instagramTags);

            return Json(content);
        }


        [HttpGet("test")]
        public IActionResult Test()
        {
            ViewBag.MachineTags = new[] { "test", "sdfsd" };
            ViewBag.InstagramTags = new[] { "sdfsdfsdf", "sdfsd" };
            return View("Post");
        }

        // POST api/<controller>/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Post(IFormFile file)
        {

            if (!Request.ContentType.Contains("multipart/form-data; boundary"))
            {
                return BadRequest("wrong contentType :'(");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No Files uploaded");
            }


            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var bytes = stream.ToArray();
                var machineTags = _taggingProvider.GetTagsForImageBytes(bytes);

                if (!machineTags.Any())
                {
                    return BadRequest("No MachineTags found :'(");
                }

                var instagramTags = _db.FindHumanoidTags(machineTags);

                ViewBag.MachineTags = machineTags;
                ViewBag.InstagramTags = instagramTags;
                return this.View();
            }
        }

    }
}
