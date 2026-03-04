namespace ClientesPedidos.Models
{
    public class PedidoListViewModel
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }   // 👈 ADICIONE ISSO

        public string NomeCliente { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public decimal Valor { get; set; }

        public DateTime DataPedido { get; set; }
    }
}