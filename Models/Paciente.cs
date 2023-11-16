using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Paciente
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public virtual ICollection<Consulta> Consultas { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        public Paciente()
        {
        }
    }
}