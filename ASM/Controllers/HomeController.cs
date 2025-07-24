using Microsoft.AspNetCore.Mvc;

namespace ASM.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}