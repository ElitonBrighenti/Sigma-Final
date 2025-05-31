using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sigma.Application.Dtos;
using Sigma.Application.Interfaces;
using Sigma.Application.Services;
using Swashbuckle.AspNetCore.Annotations;


namespace Sigma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("cadastrarusuario")]
        [SwaggerOperation(Summary = "Cadastra um novo usuário", Description = "Cria um novo usuário no sistema. É necessário informar nome de usuário, senha e role.")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Cadastrar([FromBody] UsuarioCadastroDto dto)
        {
            try
            {
                await _usuarioService.CadastrarUsuarioAsync(dto);
                return Ok("Usuário cadastrado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("buscarusuarios")]
        [SwaggerOperation(Summary = "Lista todos os usuários", Description = "Retorna todos os usuários cadastrados no sistema.")]
        [ProducesResponseType(typeof(IEnumerable<UserLoginDto>), 200)]
        public async Task<IActionResult> Listar()
        {
            var usuarios = await _usuarioService.ListarTodosAsync();
            return Ok(usuarios);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/deletarusuario")]
        [SwaggerOperation(Summary = "Remove um usuário", Description = "Remove um usuário com base no ID. Apenas Admins podem executar.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Remover(long id)
        {
            await _usuarioService.RemoverAsync(id);
            return NoContent();
        }
    }
}
