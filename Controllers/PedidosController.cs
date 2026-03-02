using ClientesPedidos.Dao;
using ClientesPedidos.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientesPedidos.Controllers
{
    public class PedidosController : Controller
    {
        private readonly PedidosDAO _dao;

        public PedidosController(PedidosDAO dao)
        {
            _dao = dao;
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

        [HttpPost]
        public IActionResult Deletar(int id)
        {
            _dao.DeletePedido(id);
            return RedirectToAction("Index");
        }
    }
}