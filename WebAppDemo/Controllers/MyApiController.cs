using System.Web.Http;

namespace WebAppDemo.Controllers
{
    public class MyController : ApiController
    {

        [HttpGet]
        public string Say()
        {
            return "";
        }
    }
}
