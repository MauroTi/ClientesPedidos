using Microsoft.AspNetCore.Mvc;
using ClientesPedidos.Dao;
using ClientesPedidos.ViewModels;
using ClientesPedidos.Models;


namespace ClientesPedidos.Controllers
{
    public class PedidosController : Controller
    {
        private readonly ClientesDAO _clientesDao;
        private readonly PedidosDAO _pedidosDao;

        public PedidosController(ClientesDAO clientesDao, PedidosDAO pedidosDao)
        {
            _clientesDao = clientesDao;
            _pedidosDao = pedidosDao;
        }

        [HttpGet]
        public JsonResult PedidosPorDia()
        {
            var dados = _pedidosDao.PedidosPorDia();

            return Json(dados);
        }

        // LISTAGEM
        public IActionResult Index()
        {
            var pedidos = _pedidosDao.GetPedidos();
            return View(pedidos);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            var model = new PedidoCreateViewModel
            {
                Clientes = _clientesDao.Listar()
            };

            return View(model);
        }

        // CREATE (POST)
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

            bool sucesso = _pedidosDao.AddPedido(pedido);

            if (sucesso)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Erro ao salvar pedido.");
            model.Clientes = _clientesDao.Listar();
            return View(model);
        }

        // EDIT (GET)
        public IActionResult Edit(int id)
        {
            var pedido = _pedidosDao.GetPedidos()
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            var model = new PedidoCreateViewModel
            {
                ClienteId = pedido.ClienteId,
                Descricao = pedido.Descricao,
                Valor = pedido.Valor,
                Clientes = _clientesDao.Listar()
            };

            return View(model);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PedidoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Clientes = _clientesDao.Listar();
                return View(model);
            }

            var pedido = new PedidoModel
            {
                Id = id,
                ClienteId = model.ClienteId,
                Descricao = model.Descricao,
                Valor = model.Valor
            };

            bool sucesso = _pedidosDao.UpdatePedido(pedido);

            if (sucesso)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Erro ao atualizar pedido.");
            model.Clientes = _clientesDao.Listar();
            return View(model);
        }

        [HttpGet]
        public JsonResult PedidosResumoPeriodo()
        {
            var dados = _pedidosDao.PedidosResumoPeriodo();

            return Json(dados);
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            bool sucesso = _pedidosDao.DeletePedido(id);

            if (!sucesso)
                ModelState.AddModelError("", "Erro ao excluir pedido.");

            return RedirectToAction(nameof(Index));
        }
    }
}