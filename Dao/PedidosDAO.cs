using ClientesPedidos.Models;
using ClientesPedidos.ViewModels;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

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
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erro AddPedido: {ex.Message}");
                return false;
            }
        }

        // LISTAR (JOIN com Clientes)
        public List<PedidoListViewModel> GetPedidos()
        {
            var list = new List<PedidoListViewModel>();

            try
            {
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
                        Valor = reader.IsDBNull("Valor") ? 0 : reader.GetDecimal("Valor"),
                        DataPedido = reader.GetDateTime("DataPedido"),
                        NomeCliente = reader.GetString("NomeCliente")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro GetPedidos: {ex.Message}");
            }

            return list;
        }

        // BUSCAR POR ID
        public PedidoModel? GetPedidoById(int id)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string sql = @"
                    SELECT Id, ClienteId, Descricao, Valor, DataPedido
                    FROM Pedidos
                    WHERE Id = @Id;";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new PedidoModel
                    {
                        Id = reader.GetInt32("Id"),
                        ClienteId = reader.GetInt32("ClienteId"),
                        Descricao = reader.GetString("Descricao"),
                        Valor = reader.IsDBNull("Valor") ? 0 : reader.GetDecimal("Valor"),
                        DataPedido = reader.GetDateTime("DataPedido")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro GetPedidoById: {ex.Message}");
                return null;
            }
        }

        // UPDATE
        public bool UpdatePedido(PedidoModel pedido)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erro UpdatePedido: {ex.Message}");
                return false;
            }
        }

        // DELETE
        public bool DeletePedido(int id)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string sql = "DELETE FROM Pedidos WHERE Id = @Id;";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro DeletePedido: {ex.Message}");
                return false;
            }
        }

        // GRÁFICO POR DIA
        public Dictionary<string, int> PedidosPorDia()
        {
            var dados = new Dictionary<string, int>();

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string sql = @"
                    SELECT DATE(DataPedido) as Dia,
                           COUNT(*) as Total
                    FROM Pedidos
                    GROUP BY DATE(DataPedido)
                    ORDER BY Dia;";

                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string dia = reader.GetDateTime("Dia")
                        .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                    int total = reader.GetInt32("Total");

                    dados.Add(dia, total);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro PedidosPorDia: {ex.Message}");
            }

            return dados;
        }

        // RESUMO PERÍODO
        public Dictionary<string, int> PedidosResumoPeriodo()
        {
            var dados = new Dictionary<string, int>();

            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string sql = @"
                    SELECT
                        SUM(CASE WHEN DATE(DataPedido) = CURDATE() THEN 1 ELSE 0 END) AS Hoje,
                        SUM(CASE WHEN YEARWEEK(DataPedido,1) = YEARWEEK(CURDATE(),1) THEN 1 ELSE 0 END) AS Semana,
                        SUM(CASE WHEN MONTH(DataPedido) = MONTH(CURDATE()) 
                                 AND YEAR(DataPedido) = YEAR(CURDATE()) THEN 1 ELSE 0 END) AS Mes,
                        SUM(CASE WHEN YEAR(DataPedido) = YEAR(CURDATE()) THEN 1 ELSE 0 END) AS Ano
                    FROM pedidos;";

                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    dados.Add("Hoje", reader.GetInt32("Hoje"));
                    dados.Add("Semana", reader.GetInt32("Semana"));
                    dados.Add("Mes", reader.GetInt32("Mes"));
                    dados.Add("Ano", reader.GetInt32("Ano"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro PedidosResumoPeriodo: {ex.Message}");
            }

            return dados;
        }
    }
}