

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace AutoTagger.UserInterface
{
    using Microsoft.AspNetCore.Mvc;

    [Route("[controller]")]
    public class IndexController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public ViewResult Index()
        {
            return this.View();
        }
    }
}
