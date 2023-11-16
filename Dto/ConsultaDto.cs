using api.Models;

namespace api.Dto
{
    public class ConsultaDto
    {
        public int ConsultaId { get; set; }
        public DateTime DataHora { get; set; }
        public int PacienteId { get; set; }
        public string NomePaciente { get; set; }
        public int MedicoId { get; set; }
        public string NomeMedico { get; set; }
        public StatusConsulta Status { get; set; }
        public string Observacoes { get; set; }
    }
}
