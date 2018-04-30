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
    using Microsoft.CodeAnalysis.Emit;
    using Newtonsoft.Json;

    [Route("[controller]")]
    public class ImageController : Controller
    {
        private readonly IAutoTaggerStorage storage;

        private readonly ITaggingProvider taggingProvider;

        public ImageController(IAutoTaggerStorage storage, ITaggingProvider taggingProvider)
        {
            this.storage      = storage;
            this.taggingProvider = taggingProvider;
        }

        [HttpPost("Link")]
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult Post(ScanLinkModel model)
        {
            var link = model.Link;

            if (string.IsNullOrEmpty(link))
            {
                return this.BadRequest("No Link set");
            }

            var machineTags = this.taggingProvider.GetTagsForImageUrl(link).ToList();

            if (!machineTags.Any())
            {
                return this.BadRequest("No MachineTags found :'(");
            }

            var content = this.FindTags(machineTags);
            content.Add("link", link);
            var json = this.Json(content);

            var debugStr = JsonConvert.SerializeObject(content);
            this.storage.Log("web_link", debugStr);

            return json;
        }

        [HttpPost("File")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (!this.Request.ContentType.Contains("multipart/form-data; boundary"))
            {
                return this.BadRequest("Wrong ContentType :'(");
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

                var content = this.FindTags(machineTags);
                var json = this.Json(content);

                var debug = content;
                var debugStr = JsonConvert.SerializeObject(debug);
                this.storage.Log("web_image", debugStr);

                return json;
            }
        }

        private Dictionary<string, object> FindTags(List<IMTag> machineTags)
        {
            var (query, instagramTags) = this.storage.FindHumanoidTags(machineTags);
            var ip = this.GetIPAddress();

            var data = new Dictionary<string, object>
            {
                { "machineTags", machineTags },
                { "instagramTags", instagramTags },
                { "query", query },
                { "ip", ip }
            };

            return data;
        }

        private string GetIPAddress()
        {
            return this.Request.HttpContext.Connection?.RemoteIpAddress?.ToString();
        }
    }
}
