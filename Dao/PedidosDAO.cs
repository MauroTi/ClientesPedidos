using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using ClientesPedidos.Models;

namespace ClientesPedidos.Dao
{
    public class PedidosDAO
    {
        private readonly Conexao _conexao;

        // ✅ Conexao agora vem por Injeção de Dependência
        public PedidosDAO(Conexao conexao)
        {
            _conexao = conexao;
        }

        // INSERT
        public bool AddPedido(PedidoModel pedido)
        {
            try
            {
                using var conn = _conexao.GetConnection();
                conn.Open();

                string sql = @"INSERT INTO pedidos (nome, tipo)
                               VALUES (@nome, @tipo)";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", pedido.nome);
                cmd.Parameters.AddWithValue("@tipo", pedido.tipo);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao inserir: " + ex.Message);
                return false;
            }

        }
        public bool AddPedido(string nome,string tipo) { 
        
                return AddPedido(new PedidoModel { nome = nome, tipo = tipo });

        }

        // UPDATE
        public bool UpdatePedido(PedidoModel pedido)
        {
            try
            {
                using var conn = _conexao.GetConnection();
                conn.Open();

                string sql = @"UPDATE pedidos 
                               SET nome=@nome, tipo=@tipo
                               WHERE id=@id";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", pedido.nome);
                cmd.Parameters.AddWithValue("@tipo", pedido.tipo);
                cmd.Parameters.AddWithValue("@id", pedido.id);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar: " + ex.Message);
                return false;
            }
        }

        // DELETE
        public bool DeletePedido(int id)
        {
            try
            {
                using var conn = _conexao.GetConnection();
                conn.Open();

                string sql = "DELETE FROM pedidos WHERE id=@id";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao deletar: " + ex.Message);
                return false;
            }
        }

        // SELECT ALL
        public List<PedidoModel> Listar()
        {
            var lista = new List<PedidoModel>();

            using var conn = _conexao.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM pedidos";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new PedidoModel
                {
                    id = reader.GetInt32("id"),
                    nome = reader["nome"].ToString(),
                    tipo = reader["tipo"].ToString()
                });
            }

            return lista;
        }

       
    }
}