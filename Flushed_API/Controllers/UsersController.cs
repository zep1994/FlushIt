using Microsoft.AspNetCore.Mvc;

namespace Flushed_API.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
