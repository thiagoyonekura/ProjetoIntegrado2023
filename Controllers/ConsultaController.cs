using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.Dto;

namespace api.Controllers
{
    [Route("api/consulta")]
    [ApiController]
    public class ConsultaController : ControllerBase
    {
        private readonly SistemaContext _context;

        public ConsultaController(SistemaContext context)
        {
            _context = context;
        }


        // GET: api/Consultas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultaDto>>> GetConsultas()
        {
            if (_context.Consultas == null)
            {
                return NotFound();
            }

            var consultas = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Include(c => c.HorarioDisponivel)
                .Select(c => new ConsultaDto
                {
                    ConsultaId = c.Id, 
                    DataHora = c.DataHora,
                    PacienteId = c.PacienteId,
                    NomePaciente = c.Paciente.Usuario.Nome,
                    MedicoId = c.MedicoId,
                    NomeMedico = c.Medico.Usuario.Nome, 
                    Status = c.Status,
                    Observacoes = c.Observacoes,
                })
                .ToListAsync();

            return Ok(consultas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConsultaDto>> GetConsulta(int id)
        {
            if (_context.Consultas == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Include(c => c.HorarioDisponivel)
                .Where(c => c.Id == id)
                .Select(c => new ConsultaDto
                {
                    ConsultaId = c.Id,
                    DataHora = c.DataHora,
                    PacienteId = c.PacienteId,
                    NomePaciente = c.Paciente.Usuario.Nome, 
                    MedicoId = c.MedicoId,
                    NomeMedico = c.Medico.Usuario.Nome, 
                    Status = c.Status,
                    Observacoes = c.Observacoes 
                })
                .FirstOrDefaultAsync();

            if (consulta == null)
            {
                return NotFound();
            }

            return consulta;
        }

        // GET: api/consulta/agendadas
        //Método GET que exibe todas as Consultas Agendadas de todos os Médicos.
        [HttpGet("agendadas")]
        public async Task<ActionResult<IEnumerable<Consulta>>> GetConsultasAgendadas()
        {
            if (_context.Consultas == null)
            {
                return NotFound();
            }
            var consultasAgendadas = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Include(c => c.HorarioDisponivel)
                .Where(c => c.Status == StatusConsulta.Agendado)
                .Select(c => new ConsultaDto
                {
                    ConsultaId = c.Id,
                    DataHora = c.DataHora,
                    PacienteId = c.PacienteId,
                    NomePaciente = c.Paciente.Usuario.Nome,
                    MedicoId = c.MedicoId,
                    NomeMedico = c.Medico.Usuario.Nome,
                    Status = c.Status,
                    Observacoes = c.Observacoes
                })
                .ToListAsync();

            return Ok(consultasAgendadas);
        }

        // GET: api/consulta/agendadas/medico/{medicoId}
        //Método GET que exibe todas as Consultas Agendadas de um Médico.
        [HttpGet("agendadas/medico/{medicoId}")]
        public async Task<ActionResult<IEnumerable<Consulta>>> GetConsultasAgendadasPorMedico(int medicoId)
        {
            if (_context.Consultas == null)
            {
                return NotFound();
            }
            var consultasAgendadas = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Include(c => c.HorarioDisponivel)
                .Where(c => c.Status == StatusConsulta.Agendado && c.MedicoId == medicoId)
                .Select(c => new ConsultaDto
                {
                    ConsultaId = c.Id,
                    DataHora = c.DataHora,
                    PacienteId = c.PacienteId,
                    NomePaciente = c.Paciente.Usuario.Nome,
                    MedicoId = c.MedicoId,
                    NomeMedico = c.Medico.Usuario.Nome,
                    Status = c.Status,
                    Observacoes = c.Observacoes
                })
                .ToListAsync();

            if (!consultasAgendadas.Any())
            {
                return NotFound($"Nenhuma consulta agendada encontrada para o médico com ID {medicoId}.");
            }

            return Ok(consultasAgendadas);
        }

        [HttpPost("CriaConsulta")]
        public async Task<ActionResult<Consulta>> CriaConsulta(CriaConsultaDto consultaDto)
        {
            if (_context == null)
            {
                return Problem("Não foi possível conectar no Banco de Dados");
            }

            // Encontra o horário disponível com base no ID fornecido
            var horario = await _context.HorariosDisponiveis.FindAsync(consultaDto.HorarioId);
            if (horario == null || !horario.Disponivel)
            {
                return BadRequest("Horário não disponível ou não encontrado.");
            }

            // Cria a consulta com as informações fornecidas
            var consulta = new Consulta
            {
                DataHora = horario.DataHoraInicio,
                Status = StatusConsulta.Agendado,
                PacienteId = consultaDto.PacienteId,
                MedicoId = consultaDto.MedicoId,
                HorarioDisponivelId = consultaDto.HorarioId,
                Observacoes = consultaDto.Observacoes,
            };

            // Atualiza o status do horário para indisponível
            horario.Disponivel = false;
            _context.Entry(horario).State = EntityState.Modified;

            // Adiciona a consulta ao contexto e salva as mudanças
            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            return Ok("Consulta agendada com sucesso.");
        }
            
        // DELETE: api/Consultas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsulta(int id)
        {
            if (_context.Consultas == null)
            {
                return NotFound();
            }
            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null)
            {
                return NotFound();
            }

            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/consulta/5/cancelar
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarConsulta(int id)
        {
            var consulta = await _context.Consultas
                                         .Include(c => c.HorarioDisponivel)
                                         .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta == null)
            {
                return NotFound();
            }

            if (consulta.Status == StatusConsulta.Cancelado || consulta.Status == StatusConsulta.Concluido)
            {
                return BadRequest("A consulta já está cancelada ou concluída.");
            }

            if ((consulta.DataHora - DateTime.Now).TotalHours < 24)
            {
                return BadRequest("A consulta não pode ser cancelada com menos de 24 horas de antecedência.");
            }

            // Cancela a consulta
            consulta.Status = StatusConsulta.Cancelado;

            // Marca o horário como disponível novamente
            var horario = consulta.HorarioDisponivel;
            if (horario != null)
            {
                horario.Disponivel = true;
                _context.Entry(horario).State = EntityState.Modified;
            }

            // Salva as mudanças no contexto
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool ConsultaExists(int id)
        {
            return (_context.Consultas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
