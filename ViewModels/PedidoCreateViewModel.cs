using ClientesPedidos.Models;

namespace ClientesPedidos.ViewModels
{
    public class PedidoCreateViewModel
    {
        public int ClienteId { get; set; }

        public string? Descricao { get; set; }

        public decimal? Valor { get; set; }

        public List<ClienteModel> Clientes { get; set; } = new();
    }
}