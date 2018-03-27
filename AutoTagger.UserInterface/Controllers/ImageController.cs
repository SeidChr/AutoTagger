namespace AutoTagger.UserInterface.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using AutoTagger.Contract;
    using AutoTagger.UserInterface.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;

    [Route("[controller]")]
    public class ImageController : Controller
    {
        private readonly IAutoTaggerDatabase db;

        private readonly ITaggingProvider taggingProvider;

        public ImageController(IAutoTaggerDatabase db, ITaggingProvider taggingProvider)
        {
            this.db = db;
            this.taggingProvider = taggingProvider;
        }

        
        [HttpPost("Link")]
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult Post(ScanLinkModel model)
        {
            

            var machineTags = this.taggingProvider.GetTagsForImageUrl(model.Link).ToList();
            

            if (!machineTags.Any())
            {
                return this.BadRequest("No MachineTags found :'(");
            }

            var instagramTags = this.db.FindHumanoidTags(machineTags);

            var content = new Dictionary<string, object>();
            content.Add("link", model.Link);
            content.Add("machineTags", machineTags);
            content.Add("instagramTags", instagramTags);
            return this.Json(content);
        }

        [HttpPost("File")]
        [ProducesResponseType(typeof(void), 200)]
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
                var bytes = stream.ToArray();
                var machineTags = this.taggingProvider.GetTagsForImageBytes(bytes).ToList();

                if (!machineTags.Any())
                {
                    return this.BadRequest("No MachineTags found :'(");
                }

                var instagramTags = this.db.FindHumanoidTags(machineTags);

                var content = new Dictionary<string, object>();
                content.Add("machineTags",   machineTags);
                content.Add("instagramTags", instagramTags);
                return this.Json(content);
            }
        }
    }
}
