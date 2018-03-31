namespace AutoTagger.UserInterface.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoTagger.Contract;
    using AutoTagger.UserInterface.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("[controller]")]
    public class ImageController : Controller
    {
        private readonly IAutoTaggerStorage repository;

        private readonly ITaggingProvider taggingProvider;

        public ImageController(IAutoTaggerStorage repository, ITaggingProvider taggingProvider)
        {
            this.repository      = repository;
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

            var instagramTags = this.repository.FindHumanoidTags(machineTags);

            var content = new Dictionary<string, object>
            {
                { "link", model.Link },
                { "machineTags", machineTags },
                { "instagramTags", instagramTags }
            };

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
                var bytes       = stream.ToArray();
                var machineTags = this.taggingProvider.GetTagsForImageBytes(bytes).ToList();

                if (!machineTags.Any())
                {
                    return this.BadRequest("No MachineTags found :'(");
                }

                var instagramTags = this.repository.FindHumanoidTags(machineTags);

                var content = new Dictionary<string, object>
                {
                    { "machineTags", machineTags },
                    { "instagramTags", instagramTags }
                };

                return this.Json(content);
            }
        }
    }
}
