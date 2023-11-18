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
    [Route("api/paciente")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly SistemaContext _context;

        public PacienteController(SistemaContext context)
        {
            _context = context;
        }

        /*método GET que recebe o ID do paciente e retorna uma lista de DTOs
        com o histórico de agendamentos desse paciente.*/
        [HttpGet("relatorio/{pacienteId}")]
        public async Task<ActionResult<IEnumerable<RelatorioAgendamentoDto>>> GetRelatorioPaciente(int pacienteId)
        {
            var paciente = await _context.Pacientes
                             .Include(p => p.Usuario)
                             .Include(p => p.Consultas)
                             .ThenInclude(c => c.Medico)
                             .ThenInclude(m => m.Usuario)
                             .FirstOrDefaultAsync(p => p.Id == pacienteId);

            if (paciente == null)
            {
                return NotFound(new { message = "Paciente não encontrado." });
            }

            var relatorio = paciente.Consultas.Select(c => new RelatorioAgendamentoDto
            {
                ConsultaId = c.Id,
                DataHora = c.DataHora,
                NomePaciente = paciente.Usuario.Nome,
                NomeMedico = c.Medico.Usuario.Nome,
                Status = c.Status
            }).ToList();

            return Ok(relatorio);
        }

        // GET: api/paciente/consultas/agendadas
        [HttpGet("consultas/agendadas")]
        public async Task<ActionResult<IEnumerable<ConsultaDto>>> GetConsultasAgendadasDeTodos()
        {
            var consultasAgendadas = await _context.Consultas
                                                    .Include(c => c.Paciente)
                                                    .Include(c => c.Paciente.Usuario)
                                                    .Include(c => c.Medico)
                                                    .Include(c => c.Medico.Usuario)
                                                    .Where(c => c.Status == StatusConsulta.Agendado)
                                                    .Select(c => new ConsultaDto
                                                    {
                                                        ConsultaId = c.Id,
                                                        DataHora = c.DataHora,
                                                        NomePaciente = c.Paciente.Usuario.Nome,
                                                        NomeMedico = c.Medico.Usuario.Nome,
                                                        Status = c.Status,
                                                        Observacoes = c.Observacoes
                                                    })
                                                    .ToListAsync();

            return Ok(consultasAgendadas);
        }

        // GET: api/paciente/{pacienteId}/consultas/agendadas
        [HttpGet("{pacienteId}/consultas/agendadas")]
        public async Task<ActionResult<IEnumerable<ConsultaDto>>> GetConsultasAgendadasDoPaciente(int pacienteId)
        {
            var consultasAgendadas = await _context.Consultas
                                                    .Include(c => c.Paciente)
                                                    .Include(c => c.Paciente.Usuario)
                                                    .Include(c => c.Medico)
                                                    .Include(c => c.Medico.Usuario)
                                                    .Where(c => c.PacienteId == pacienteId && c.Status == StatusConsulta.Agendado)
                                                    .Select(c => new ConsultaDto
                                                    {
                                                        ConsultaId = c.Id,
                                                        DataHora = c.DataHora,
                                                        NomePaciente = c.Paciente.Usuario.Nome,
                                                        NomeMedico = c.Medico.Usuario.Nome,
                                                        Status = c.Status,
                                                        Observacoes = c.Observacoes
                                                    })
                                                    .ToListAsync();

            if (!consultasAgendadas.Any())
            {
                return NotFound("Nenhuma consulta agendada encontrada para este paciente.");
            }

            return Ok(consultasAgendadas);
        }

        // GET: api/Pacientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes()
        {
            if (_context.Pacientes == null)
            {
                return NotFound();
            }
            return await _context.Pacientes
                .Include(p => p.Usuario)
                .ToListAsync();
        }

        // GET: api/Pacientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Paciente>> GetPaciente(int id)
        {
            if (_context.Pacientes == null)
            {
                return NotFound();
            }
            var paciente = await _context.Pacientes                            
                            .Include(p => p.Usuario)
                            .FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            return paciente;
        }

        // POST: api/paciente/CadastrarComUsuario/{usuarioId}
        [HttpPost("CadastrarComUsuario/{usuarioId}")]
        public async Task<ActionResult<Paciente>> CadastrarComUsuario(int usuarioId)
        {
            // Verifica se o usuário existe
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Verifica se já existe um paciente associado a esse usuário
            var pacienteExistente = await _context.Pacientes
                                                  .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
            if (pacienteExistente != null)
            {
                return BadRequest("Já existe um paciente associado a este usuário.");
            }

            // Cria um novo paciente associado ao usuário existente
            var novoPaciente = new Paciente
            {
                UsuarioId = usuario.Id
            };

            _context.Pacientes.Add(novoPaciente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaciente", new { id = novoPaciente.Id }, novoPaciente); // Assumindo que existe um método GetPaciente
        }

        // PUT: api/Pacientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaciente(int id, Usuario paciente)
        {
            if (id != paciente.Id)
            {
                return BadRequest();
            }

            _context.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PacienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Pacientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            if (_context.Pacientes == null)
            {
                return NotFound();
            }
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PacienteExists(int id)
        {
            return (_context.Pacientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
