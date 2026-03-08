using ClientesPedidos.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClientesPedidos.ViewModels
{
    public class PedidoCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(200)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999)]
        public decimal Valor { get; set; }

        public List<ClienteModel> Clientes { get; set; } = new();
    }
}