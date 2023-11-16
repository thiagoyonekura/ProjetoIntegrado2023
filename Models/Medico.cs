using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Medico
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O CRM é obrigatório.")]
        public string CRM { get; set; } // CRM como identificador único para médicos no Brasil

        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        public string Especialidade { get; set; }

        public virtual ICollection<Consulta> Consultas { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        
        public Medico() 
        {
            // Inicializar a coleção para evitar problemas de referência nula
            Consultas = new HashSet<Consulta>();
        }
    }
}
