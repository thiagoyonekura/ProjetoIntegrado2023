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
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly SistemaContext _context;

        public UsuarioController(SistemaContext context)
        {
            _context = context;
        }

        // GET: api/Usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
          if (_context.Usuarios == null)
          {
              return NotFound();
          }
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("{id}")]
        public IActionResult GetUsuarioById(int id)
        {
            Usuario usuario = _context.Usuarios.FirstOrDefault(usuario => usuario.Id == id);
            if (usuario != null)
            {
                return Ok(usuario);
            }
            return NotFound();
        }

        // Gera um relatório de histórico de agendamento de todos os pacientes
        [HttpGet("relatorio-agendamentos")]
        public async Task<ActionResult<IEnumerable<RelatorioAgendamentoDto>>> GetRelatorioAgendamentos()
        {
            var consultas = await _context.Consultas
                                          .Include(c => c.Paciente)
                                          .ThenInclude(p => p.Usuario)
                                          .Include(c => c.Medico)
                                          .ThenInclude(m => m.Usuario)
                                          .ToListAsync();

            var relatorio = consultas.Select(c => new RelatorioAgendamentoDto
            {
                ConsultaId = c.Id,
                DataHora = c.DataHora,
                NomePaciente = c.Paciente.Usuario.Nome,
                NomeMedico = c.Medico.Usuario.Nome,
                Status = c.Status
            }).ToList();

            return Ok(relatorio);
        }

        // PUT: api/Usuario/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // POST: api/usuario
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
          if (_context.Usuarios == null)
          {
              return Problem("Entity set 'SistemaContext.Usuarios'  is null.");
          }
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuarioById), new { id = usuario.Id }, usuario);
        }

        // POST: api/usuario/UsuarioEPaciente
        [HttpPost("UsuarioEPaciente")]
       
        public async Task<IActionResult> CadastroUsuarioEPaciente([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Adiciona o usuário ao contexto
            _context.Add(usuario);

            // Salva as mudanças no contexto, efetivamente persistindo o usuário e gerando o ID
            await _context.SaveChangesAsync();

            // Cria um novo paciente associado ao usuário
            var paciente = new Paciente
            {
                UsuarioId = usuario.Id 
            };

            // Adiciona o paciente ao contexto
            _context.Add(paciente);

            // Salva as mudanças no contexto para persistir o paciente
            await _context.SaveChangesAsync();

            // Retorna uma resposta com os dados do usuário e paciente criados
            return Ok(new { UsuarioId = usuario.Id, PacienteId = paciente.Id });
        }



        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("login")]
        public IActionResult LoginUsuario([FromBody] LoginDto loginRequest)
        {
            Usuario usuario = _context.Usuarios.FirstOrDefault(usuario => usuario.Email == loginRequest.Email &
            usuario.Senha == loginRequest.Senha);

            if (usuario != null)
            {

                Paciente paciente = _context.Pacientes.FirstOrDefault(h => h.UsuarioId == usuario.Id);

                return Ok(new { usuario, paciente });
            }
            return NotFound("Usuario não localizado ou senha inválida!");
        }
        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
