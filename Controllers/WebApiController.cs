using Microsoft.AspNetCore.Mvc;

namespace Music_Club.Controllers
{
    public class WebApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
