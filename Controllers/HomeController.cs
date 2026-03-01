using ClientesPedidos.Dao;
using ClientesPedidos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClientesPedidos.Controllers
{
    public class HomeController : Controller
    {
        private readonly PedidosDAO _dao;

        private readonly ILogger<HomeController> _logger;

        public HomeController(PedidosDAO dao, ILogger<HomeController> logger)
        {
            _dao = dao;
            _logger = logger;
        }
        


        public IActionResult Index()
        {
            var lista = _dao.Listar();
            return View(lista);
        }
       
        [HttpPost]
        public IActionResult Criar(string nome, string tipo)
        {
            _dao.AddPedido(nome, tipo);
            return RedirectToAction("Index");
        }

        public IActionResult Deletar(int id)
        {
            _dao.DeletePedido(id);
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
