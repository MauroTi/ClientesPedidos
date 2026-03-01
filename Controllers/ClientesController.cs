using Microsoft.AspNetCore.Mvc;
using ClientesPedidos.Dao;
using ClientesPedidos.Models;

namespace ClientesPedidos.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ClientesDAO _dao;

        public ClientesController(ClientesDAO dao)
        {
            _dao = dao;
        }

        public IActionResult Index()
        {
            var lista = _dao.Listar();
            return View(lista);
        }

        [HttpPost]
        public IActionResult Criar(string nome, string telefone)
        {
            _dao.AddCliente(nome, telefone);
            return RedirectToAction("Index");
        }

        public IActionResult Deletar(int id)
        {
            _dao.DeleteCliente(id);
            return RedirectToAction("Index");
        }
    }
}