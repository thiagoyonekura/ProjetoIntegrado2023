namespace api.Dto
{
    public class HorariosIndisponiveisDto
    {
        public int MedicoId { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
       public HorariosIndisponiveisDto()
        {
        }
    }
}
