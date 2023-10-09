using Microsoft.AspNetCore.Mvc;

namespace WebServer.Controllers
{
    /// <summary>
    /// Базовый контроллер проекта
    /// </summary>
    public abstract class BaseController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
