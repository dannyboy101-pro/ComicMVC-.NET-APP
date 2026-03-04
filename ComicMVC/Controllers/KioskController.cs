using Microsoft.AspNetCore.Mvc;

namespace ComicMVC.Controllers
{
    public class KioskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}