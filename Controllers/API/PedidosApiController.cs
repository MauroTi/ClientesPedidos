using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ClientesPedidos.Controllers.API
{
    [Route("api/Pedidos")]
    [ApiController]
    public class PedidosApiController : ControllerBase
    {
        private readonly string connectionString = "server=localhost;database=clientespedidos;uid=root;";

        private int GetIntSafe(MySqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            return reader.IsDBNull(index) ? 0 : reader.GetInt32(index);
        }

        private decimal GetDecimalSafe(MySqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            return reader.IsDBNull(index) ? 0 : reader.GetDecimal(index);
        }

        private string GetStringSafe(MySqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            return reader.IsDBNull(index) ? "" : reader.GetString(index);
        }

        private DateTime? GetDateTimeNullableSafe(MySqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            return reader.IsDBNull(index) ? (DateTime?)null : reader.GetDateTime(index);
        }

        // GET: api/Pedidos
        [HttpGet]
        public IActionResult GetPedidos()
        {
            var pedidos = new List<object>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT p.Id, p.ClienteId, c.Nome AS NomeCliente,
                           p.Descricao, p.Valor, p.DataPedido, p.DataHoraPedido
                    FROM pedidos p
                    LEFT JOIN clientes c ON p.ClienteId = c.Id
                    ORDER BY p.Id";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pedidos.Add(new
                        {
                            Id = GetIntSafe(reader, "Id"),
                            ClienteId = GetIntSafe(reader, "ClienteId"),
                            NomeCliente = GetStringSafe(reader, "NomeCliente"),
                            Descricao = GetStringSafe(reader, "Descricao"),
                            Valor = GetDecimalSafe(reader, "Valor"),
                            DataPedido = GetDateTimeNullableSafe(reader, "DataPedido"),
                            DataHoraPedido = GetDateTimeNullableSafe(reader, "DataHoraPedido")
                        });
                    }
                }
            }

            return Ok(pedidos);
        }

        // GET: api/Pedidos/{id}
        [HttpGet("{id}")]
        public IActionResult GetPedido(int id)
        {
            object pedido = null;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT p.Id, p.ClienteId, c.Nome AS NomeCliente,
                           p.Descricao, p.Valor, p.DataPedido, p.DataHoraPedido
                    FROM pedidos p
                    LEFT JOIN clientes c ON p.ClienteId = c.Id
                    WHERE p.Id = @Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pedido = new
                            {
                                Id = GetIntSafe(reader, "Id"),
                                ClienteId = GetIntSafe(reader, "ClienteId"),
                                NomeCliente = GetStringSafe(reader, "NomeCliente"),
                                Descricao = GetStringSafe(reader, "Descricao"),
                                Valor = GetDecimalSafe(reader, "Valor"),
                                DataPedido = GetDateTimeNullableSafe(reader, "DataPedido"),
                                DataHoraPedido = GetDateTimeNullableSafe(reader, "DataHoraPedido")
                            };
                        }
                    }
                }
            }

            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }

        // GET: api/Pedidos/Cliente/{clienteId}
        [HttpGet("Cliente/{clienteId}")]
        public IActionResult GetPedidosPorCliente(int clienteId)
        {
            var pedidos = new List<object>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT p.Id, p.ClienteId, c.Nome AS NomeCliente,
                           p.Descricao, p.Valor, p.DataPedido, p.DataHoraPedido
                    FROM pedidos p
                    LEFT JOIN clientes c ON p.ClienteId = c.Id
                    WHERE p.ClienteId = @ClienteId
                    ORDER BY p.Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", clienteId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pedidos.Add(new
                            {
                                Id = GetIntSafe(reader, "Id"),
                                ClienteId = GetIntSafe(reader, "ClienteId"),
                                NomeCliente = GetStringSafe(reader, "NomeCliente"),
                                Descricao = GetStringSafe(reader, "Descricao"),
                                Valor = GetDecimalSafe(reader, "Valor"),
                                DataPedido = GetDateTimeNullableSafe(reader, "DataPedido"),
                                DataHoraPedido = GetDateTimeNullableSafe(reader, "DataHoraPedido")
                            });
                        }
                    }
                }
            }

            return Ok(pedidos);
        }

        // POST: api/Pedidos
        [HttpPost]
        public IActionResult CreatePedido([FromBody] dynamic pedido)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO pedidos (ClienteId, Descricao, Valor)
                    VALUES (@ClienteId, @Descricao, @Valor)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", pedido?.ClienteId != null ? (int)pedido.ClienteId : 0);
                    cmd.Parameters.AddWithValue("@Descricao", pedido?.Descricao?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Valor", pedido?.Valor != null ? (decimal)pedido.Valor : 0);

                    cmd.ExecuteNonQuery();
                }
            }

            return Ok(new { mensagem = "Pedido criado com sucesso." });
        }

        // PUT: api/Pedidos/{id}
        [HttpPut("{id}")]
        public IActionResult UpdatePedido(int id, [FromBody] dynamic pedido)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    UPDATE pedidos
                    SET ClienteId = @ClienteId,
                        Descricao = @Descricao,
                        Valor = @Valor
                    WHERE Id = @Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@ClienteId", pedido?.ClienteId != null ? (int)pedido.ClienteId : 0);
                    cmd.Parameters.AddWithValue("@Descricao", pedido?.Descricao?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Valor", pedido?.Valor != null ? (decimal)pedido.Valor : 0);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                        return NotFound();
                }
            }

            return Ok(new { mensagem = "Pedido atualizado com sucesso." });
        }

        // DELETE: api/Pedidos/{id}
        [HttpDelete("{id}")]
        public IActionResult DeletePedido(int id)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"DELETE FROM pedidos WHERE Id = @Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                        return NotFound();
                }
            }

            return Ok(new { mensagem = "Pedido excluído com sucesso." });
        }
    }
}