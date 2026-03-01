using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using ClientesPedidos.Models;


namespace ClientesPedidos.Dao
{
    public class ClientesDAO
    {
        private readonly Conexao _conexao;

        public ClientesDAO(Conexao conexao)
        {
            _conexao = conexao;
        }

        // INSERT
        public bool AddCliente(string nome, string telefone)
        {
            try
            {
                using var conn = _conexao.GetConnection();
                conn.Open();

                string sql = @"INSERT INTO clientes (nome, telefone)
                               VALUES (@nome, @telefone)";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@telefone", telefone);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao inserir cliente: " + ex.Message);
                return false;
            }
        }

        // UPDATE
        public bool UpdateCliente(int id, string nome, string telefone)
        {
            try
            {
                using var conn = _conexao.GetConnection();
                conn.Open();

                string sql = @"UPDATE clientes
                               SET nome=@nome, telefone=@telefone
                               WHERE id=@id";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@telefone", telefone);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar cliente: " + ex.Message);
                return false;
            }
        }

        // DELETE
        public bool DeleteCliente(int id)
        {
            try
            {
                using var conn = _conexao.GetConnection();
                conn.Open();

                string sql = "DELETE FROM clientes WHERE id=@id";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao deletar cliente: " + ex.Message);
                return false;
            }
        }

        // SELECT ALL
        public List<ClienteModel> Listar()
        {
            var lista = new List<ClienteModel>();

            using var conn = _conexao.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM clientes";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new ClienteModel
                {
                    id = Convert.ToInt32(reader["id"]),
                    nome = reader["nome"]?.ToString() ?? string.Empty,
                    telefone = reader["telefone"]?.ToString() ?? string.Empty
                });
            }

            return lista;
        }
    }
}