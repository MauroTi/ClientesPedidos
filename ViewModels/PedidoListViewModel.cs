namespace ClientesPedidos.ViewModels
{
    public class PedidoListViewModel
    {
        public int Id { get; set; }

        public string? NomeCliente { get; set; }

        public string? Descricao { get; set; }

        public decimal? Valor { get; set; }

        public DateTime DataPedido { get; set; }
    }
}