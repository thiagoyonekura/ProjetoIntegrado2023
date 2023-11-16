using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Consulta
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "A data da consulta é obrigatória.")]
        public DateTime DataHora { get; set; }
        [Required]
        public StatusConsulta Status { get; set; }

        [Required(ErrorMessage = "O identificador do paciente é obrigatório.")]
        [ForeignKey("Paciente")]
        public int PacienteId { get; set; }
        public virtual Paciente Paciente { get; set; }

        [Required(ErrorMessage = "O identificador do médico é obrigatório.")]
        [ForeignKey("Medico")]
        public int MedicoId { get; set; }
        public virtual Medico Medico { get; set; }

        [Required]
        [ForeignKey("HorarioDisponivel")]
        public int HorarioDisponivelId { get; set; }
        public virtual HorarioDisponivel HorarioDisponivel { get; set; }

        // Campo adicional para observações sobre a consulta
        [StringLength(255)]
        public string Observacoes { get; set; }


        public Consulta() 
        { 
        }

        public Consulta(int pacienteId, int medicoId, int horarioDisponivelId, DateTime dataHora, string observacoes)
        {
            PacienteId = pacienteId;
            MedicoId = medicoId;
            HorarioDisponivelId = horarioDisponivelId;
            // Garante que DataHora esteja em UTC
            DataHora = dataHora.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(dataHora, DateTimeKind.Utc)
                        : dataHora.ToUniversalTime();
            Observacoes = observacoes;
            Status = StatusConsulta.Agendado; // Por padrão, o status é 'Agendado'
        }

        // Método para cancelar o agendamento
        public bool CancelarAgendamento()
        {
            // Verifica se a consulta já está cancelado ou concluído
            if (Status == StatusConsulta.Cancelado || Status == StatusConsulta.Concluido)
            {
                return false; // Não é possível cancelar uma consulta já cancelado ou concluído
            }

            // Verificar se o cancelamento está sendo feito com antecedência mínima
            // A política da clínica seja permitir cancelamentos até 24 horas antes da consulta
            if ((DataHora - DateTime.UtcNow).TotalHours < 24)
            {
                return false; // Não cancela se estiver a menos de 24 horas da consulta
            }

            // Atualizar o status do agendamento para "Cancelado"
            Status = StatusConsulta.Cancelado;

            // Indicar que o cancelamento foi bem-sucedido
            return true;

        }

        // Método para agendar uma consulta
        public static bool AgendarConsulta(DbContext context, int medicoId, int pacienteId, DateTime dataHora, int horarioDisponivelId, string observacoes)
        {
            // Aqui, você pode adicionar lógica para verificar a disponibilidade do médico e horário,
            // similar ao exemplo de código fornecido anteriormente.

            // Após verificar a disponibilidade, criar e agendar uma nova consulta:
            var novaConsulta = new Consulta(pacienteId, medicoId, horarioDisponivelId, dataHora, observacoes);
            context.Add(novaConsulta);
            context.SaveChanges(); // Salva as mudanças no banco de dados

            return true; // Retorna true indicando que o agendamento foi bem-sucedido
        }

    }
}
