using Demo.Models;
using Demo.Services;
using EasyDo.Cache;
using System.Web.Mvc;

namespace WebAppDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITeacherService TeacherService;
        private readonly ICache Cache;

        public HomeController(ITeacherService TeacherService, ICache Cache)
        {
            this.TeacherService = TeacherService;
            this.Cache = Cache;
        }

        public ActionResult Index()
        {
         
            return View();
        }

        public ActionResult About()
        {
            TeacherService.Insert(new Teacher { TName="小明老师" });
            return View();
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";
            //var x= TeacherService.GetById("59700c72e040861730bf4a95");
            var x = Cache.Get<Teacher>("59769ffce040862234651015", () => { return TeacherService.GetById("59769ffce040862234651015"); }, 20);
            return View();
        }
    }
}