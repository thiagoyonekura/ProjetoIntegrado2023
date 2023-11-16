using api.Models;

namespace api.Dto
{
    public class RelatorioAgendamentoDto
    {
        public int ConsultaId { get; set; }
        public DateTime DataHora { get; set; }
        public string NomePaciente { get; set; }
        public string NomeMedico { get; set; }
        public StatusConsulta Status { get; set; }
    }
}
