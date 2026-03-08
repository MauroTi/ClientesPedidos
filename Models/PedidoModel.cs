namespace ClientesPedidos.Models
{
    public class PedidoModel
    {
        public int Id { get; set; }              // corresponde a `Id` no banco
        public int ClienteId { get; set; }       // corresponde a `ClienteId` (FK)
        public string? Descricao { get; set; }   // corresponde a `Descricao`
        public decimal? Valor { get; set; }      // corresponde a `Valor`
        public DateTime DataPedido { get; set; } // corresponde a `DataPedido`

        public List<ClienteModel> Clientes { get; } = new();

        // opcional para facilitar a View
        public string? NomeCliente { get; set; } // para exibir na tabela com JOIN
    }
}