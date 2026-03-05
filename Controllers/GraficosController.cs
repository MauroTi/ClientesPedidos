using Microsoft.AspNetCore.Mvc;

namespace ClientesPedidos.Controllers
{
    public class GraficosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}