namespace api.Dto
{
    public class CriaConsultaDto
    {
        public int PacienteId { get; set; }

        public int MedicoId { get; set; }

        public int HorarioId { get; set; }
        public string Observacoes { get; set; }

    }
}
