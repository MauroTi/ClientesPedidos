using MySql.Data.MySqlClient;
using System.Collections.Generic;
using ClientesPedidos.Models;


namespace ClientesPedidos.Dao
{
    public class PedidosDAO
    {
        private readonly string _connectionString;

        public PedidosDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        // INSERT
        public bool AddPedido(PedidoModel pedido)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO Pedidos (ClienteId, Descricao, Valor)
                VALUES (@ClienteId, @Descricao, @Valor);";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
            cmd.Parameters.AddWithValue("@Descricao", pedido.Descricao ?? string.Empty);
            cmd.Parameters.AddWithValue("@Valor", pedido.Valor ?? 0);

            return cmd.ExecuteNonQuery() > 0;
        }

        // LISTAR (JOIN com Clientes para trazer Nome)
        public List<PedidoListViewModel> GetPedidos()
        {
            var list = new List<PedidoListViewModel>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT p.Id, p.ClienteId, p.Descricao, p.Valor, p.DataPedido,
                       c.Nome AS NomeCliente
                FROM Pedidos p
                JOIN Clientes c ON p.ClienteId = c.Id;";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new PedidoListViewModel
                {
                    Id = reader.GetInt32("Id"),
                    ClienteId = reader.GetInt32("ClienteId"),
                    Descricao = reader.GetString("Descricao"),
                    Valor = reader.GetDecimal("Valor"),
                    DataPedido = reader.GetDateTime("DataPedido"),
                    NomeCliente = reader.GetString("NomeCliente")
                });
            }

            return list;
        }

        // UPDATE
        public bool UpdatePedido(PedidoModel pedido)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                UPDATE Pedidos
                SET ClienteId = @ClienteId,
                    Descricao = @Descricao,
                    Valor = @Valor
                WHERE Id = @Id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
            cmd.Parameters.AddWithValue("@Descricao", pedido.Descricao ?? string.Empty);
            cmd.Parameters.AddWithValue("@Valor", pedido.Valor ?? 0);
            cmd.Parameters.AddWithValue("@Id", pedido.Id);

            return cmd.ExecuteNonQuery() > 0;
        }

        // DELETE
        public bool DeletePedido(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = "DELETE FROM Pedidos WHERE Id = @Id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}