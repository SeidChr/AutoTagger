namespace AutoTagger.UserInterface
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoTagger.Contract;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("[controller]")]
    public class ImageController : Controller
    {
        private readonly IAutoTaggerDatabase db;

        private readonly ITaggingProvider taggingProvider;

        public ImageController(IAutoTaggerDatabase db, ITaggingProvider taggingProvider)
        {
            this.db              = db;
            this.taggingProvider = taggingProvider;
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromForm] string link)
        {
            var content = new Dictionary<string, object>();
            content.Add("link", link);

            var machineTags = this.taggingProvider.GetTagsForImageUrl(link).ToList();
            content.Add("machineTags", machineTags);

            if (!machineTags.Any())
            {
                return this.BadRequest("No MachineTags found :'(");
            }

            var instagramTags = this.db.FindHumanoidTags(machineTags);
            content.Add("instagramTags", instagramTags);

            return this.Json(content);
        }

        // POST api/<controller>/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (!this.Request.ContentType.Contains("multipart/form-data; boundary"))
            {
                return this.BadRequest("wrong contentType :'(");
            }

            if (file == null || file.Length == 0)
            {
                return this.BadRequest("No Files uploaded");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var bytes       = stream.ToArray();
                var machineTags = this.taggingProvider.GetTagsForImageBytes(bytes).ToList();

                if (!machineTags.Any())
                {
                    return this.BadRequest("No MachineTags found :'(");
                }

                var instagramTags = this.db.FindHumanoidTags(machineTags);

                this.ViewBag.MachineTags   = machineTags;
                this.ViewBag.InstagramTags = instagramTags;
                return this.View();
            }
        }
    }
}
