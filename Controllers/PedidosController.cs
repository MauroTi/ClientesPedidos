using Microsoft.AspNetCore.Mvc;
using ClientesPedidos.Dao;
using ClientesPedidos.ViewModels;
using ClientesPedidos.Models;
using System.Collections.Generic;
using System;

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
            var dados = _pedidosDao.PedidosPorDia() ?? new Dictionary<string, int>();
            return Json(dados);
        }

        public IActionResult Index()
        {
            var pedidos = _pedidosDao.GetPedidos();
            return View(pedidos);
        }

        public IActionResult Create()
        {
            var model = new PedidoCreateViewModel
            {
                Clientes = _clientesDao.Listar() ?? new List<ClienteModel>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PedidoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Clientes = _clientesDao.Listar() ?? new List<ClienteModel>();
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
            model.Clientes = _clientesDao.Listar() ?? new List<ClienteModel>();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var pedido = _pedidosDao.GetPedidoById(id);

            if (pedido == null)
                return NotFound();

            var model = new PedidoCreateViewModel
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                Descricao = pedido.Descricao ?? string.Empty,
                Valor = pedido.Valor ?? 0,
                Clientes = _clientesDao.Listar() ?? new List<ClienteModel>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PedidoCreateViewModel model)
        {
            if (model == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Clientes = _clientesDao.Listar() ?? new List<ClienteModel>();
                return View(model);
            }

            var pedido = new PedidoModel
            {
                Id = model.Id,
                ClienteId = model.ClienteId,
                Descricao = model.Descricao,
                Valor = model.Valor
            };

            bool sucesso = _pedidosDao.UpdatePedido(pedido);

            if (sucesso)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Erro ao atualizar pedido.");
            model.Clientes = _clientesDao.Listar() ?? new List<ClienteModel>();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            bool sucesso = _pedidosDao.DeletePedido(id);

            if (!sucesso)
            {
                TempData["Erro"] = "Não foi possível excluir o pedido.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult PedidosResumoPeriodo()
        {
            var dados = _pedidosDao.PedidosResumoPeriodo() ?? new Dictionary<string, int>();
            return Json(dados);
        }
    }
}