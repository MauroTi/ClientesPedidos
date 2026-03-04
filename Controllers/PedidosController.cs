using ClientesPedidos.Dao;
using ClientesPedidos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClientesPedidos.ViewModels;

namespace ClientesPedidos.Controllers
{
    public class PedidosController : Controller
    {
        private readonly PedidosDAO _dao;
        private readonly ClientesDAO _clientesDao;

        public PedidosController(PedidosDAO dao, ClientesDAO clientesDao)
        {
            _dao = dao;
            _clientesDao = clientesDao;
        }

        // LISTAGEM
        public IActionResult Index()
        {
            var lista = _dao.GetPedidos();
            return View(lista);
        }

        // FORMULÁRIO DE CRIAÇÃO
        public IActionResult Create()
        {
            var model = new PedidoCreateViewModel
            {
                Clientes = _clientesDao.Listar()
            };

            return View(model);
        }

        // SALVAR NOVO PEDIDO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PedidoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Clientes = _clientesDao.Listar();
                return View(model);
            }

            var pedido = new PedidoModel
            {
                ClienteId = model.ClienteId,
                Descricao = model.Descricao,
                Valor = model.Valor,
                DataPedido = DateTime.Now
            };

            _dao.AddPedido(pedido);

            return RedirectToAction("Index");
        }

        // DELETAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deletar(int id)
        {
            _dao.DeletePedido(id);
            return RedirectToAction("Index");
        }
    }
}