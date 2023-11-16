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
    [Route("api/medico")]
    [ApiController]
    public class MedicoController : ControllerBase
    {
        private readonly SistemaContext _context;

        public MedicoController(SistemaContext context)
        {
            _context = context;
        }
             
        //Método GET que exibe as informações Nome, CRM e Especialidade de todos os médicos.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicoDto>>> GetMedicosDto()
        {
            if (_context.Medicos == null)
            {
                return NotFound();
            }
            var medicos = await _context.Medicos
                                        .Include(m => m.Usuario)
                                        .Select(m => new MedicoDto
                                        {
                                            Id = m.Id,
                                            Nome = m.Usuario.Nome,
                                            CRM = m.CRM,
                                            Especialidade = m.Especialidade                                            
                                        })
                                        .ToListAsync();

            return Ok(medicos);
        }
        
        // Método GET que exibe as informações Nome, CRM e Especialidade de um médico específico.
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicoDto>> GetMedicoDto(int id)
        {
            if (_context.Medicos == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos
                                       .Include(m => m.Usuario)
                                       .Where(m => m.Id == id)
                                       .Select(m => new MedicoDto
                                       {
                                           Id = m.Id,
                                           Nome = m.Usuario.Nome,
                                           CRM = m.CRM,
                                           Especialidade = m.Especialidade
                                       })
                                       .FirstOrDefaultAsync();

            if (medico == null)
            {
                return NotFound($"Médico com ID {id} não encontrado.");
            }

            return Ok(medico);
        }

        // Método PUT para atualizar um médico existente
        [HttpPut("{medicoId}")]
        public async Task<IActionResult> AtualizaMedico(int medicoId, [FromBody] CriaMedicoDto medicoDto)
        {
            if (medicoDto == null)
            {
                return BadRequest("Dados do médico são obrigatórios.");
            }

            var medicoExistente = await _context.Medicos.FindAsync(medicoId);
            if (medicoExistente == null)
            {
                return NotFound($"Médico com ID {medicoId} não encontrado.");
            }

            medicoExistente.CRM = medicoDto.CRM;
            medicoExistente.Especialidade = medicoDto.Especialidade;
            medicoExistente.UsuarioId = medicoDto.UsuarioId;

            _context.Entry(medicoExistente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicoExists(medicoId))
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


        //Método POST para cadastrar um novo médico
        [HttpPost]
        [Route("CriaMedico")]
        public IActionResult CriaMedico([FromBody] CriaMedicoDto medicoDto)
        {
            if (medicoDto == null)
            {
                return BadRequest("Dados do médico são obrigatórios.");
            }

            var medico = new Medico
            {
                CRM = medicoDto.CRM,
                Especialidade = medicoDto.Especialidade,
                UsuarioId = medicoDto.UsuarioId
            };

            _context.Set<Medico>().Add(medico);
            _context.SaveChanges();

            return CreatedAtAction(nameof(CriaMedico), new { id = medico.Id }, medico);
        }

        // DELETE: api/Medicos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedico(int id)
        {
            if (_context.Medicos == null)
            {
                return NotFound();
            }
            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null)
            {
                return NotFound();
            }

            _context.Medicos.Remove(medico);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicoExists(int id)
        {
            return (_context.Medicos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
