using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ClientesPedidos.Controllers.API
{
    [Route("api/Clientes")]
    [ApiController]
    public class ClientesApiController : ControllerBase
    {
        private readonly string connectionString = "server=localhost;database=clientespedidos;uid=root;";

        private int GetIntSafe(MySqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            return reader.IsDBNull(index) ? 0 : reader.GetInt32(index);
        }

        private string GetStringSafe(MySqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            return reader.IsDBNull(index) ? "" : reader.GetString(index);
        }

        // GET: api/Clientes
        [HttpGet]
        public IActionResult GetClientes()
        {
            var clientes = new List<object>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"SELECT Id, Nome, Email, Telefone FROM clientes ORDER BY Id";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(new
                        {
                            Id = GetIntSafe(reader, "Id"),
                            Nome = GetStringSafe(reader, "Nome"),
                            Email = GetStringSafe(reader, "Email"),
                            Telefone = GetStringSafe(reader, "Telefone")
                        });
                    }
                }
            }

            return Ok(clientes);
        }

        // GET: api/Clientes/{id}
        [HttpGet("{id}")]
        public IActionResult GetCliente(int id)
        {
            object cliente = null;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"SELECT Id, Nome, Email, Telefone FROM clientes WHERE Id = @Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = new
                            {
                                Id = GetIntSafe(reader, "Id"),
                                Nome = GetStringSafe(reader, "Nome"),
                                Email = GetStringSafe(reader, "Email"),
                                Telefone = GetStringSafe(reader, "Telefone")
                            };
                        }
                    }
                }
            }

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        // POST: api/Clientes
        [HttpPost]
        public IActionResult CreateCliente([FromBody] dynamic cliente)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"INSERT INTO clientes (Nome, Email, Telefone)
                               VALUES (@Nome, @Email, @Telefone)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nome", cliente?.Nome?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Email", cliente?.Email?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Telefone", cliente?.Telefone?.ToString() ?? "");

                    cmd.ExecuteNonQuery();
                }
            }

            return Ok(new { mensagem = "Cliente criado com sucesso." });
        }

        // PUT: api/Clientes/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCliente(int id, [FromBody] dynamic cliente)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"UPDATE clientes
                               SET Nome = @Nome,
                                   Email = @Email,
                                   Telefone = @Telefone
                               WHERE Id = @Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Nome", cliente?.Nome?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Email", cliente?.Email?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@Telefone", cliente?.Telefone?.ToString() ?? "");

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                        return NotFound();
                }
            }

            return Ok(new { mensagem = "Cliente atualizado com sucesso." });
        }

        // DELETE: api/Clientes/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCliente(int id)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"DELETE FROM clientes WHERE Id = @Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                        return NotFound();
                }
            }

            return Ok(new { mensagem = "Cliente excluído com sucesso." });
        }
    }
}