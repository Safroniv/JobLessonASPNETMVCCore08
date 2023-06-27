using Microsoft.AspNetCore.Mvc;

namespace JobLessonASPNETMVCCore08v01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
