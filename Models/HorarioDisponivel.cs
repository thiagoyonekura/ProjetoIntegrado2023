using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class HorarioDisponivel
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Medico")]
        public int MedicoId { get; set; }

        [Required]
        public DateTime DataHoraInicio { get; set; }

        [Required]
        public DateTime DataHoraFim { get; set; }

        // Quando um novo HorarioDisponivel é criado, ele é considerado disponível por padrão
        public bool Disponivel { get; set; } = true;

        public HorarioDisponivel()
        {
        }

    }
}