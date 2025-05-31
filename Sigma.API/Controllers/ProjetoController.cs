using Microsoft.AspNetCore.Mvc;
using Sigma.Application.Dtos;
using Sigma.Application.Interfaces;
using Sigma.Domain.Dtos;
using Sigma.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;


namespace Sigma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetoController : ControllerBase
    {
        private readonly IProjetoService _projetoService;

        public ProjetoController(IProjetoService projetoService)
        {
            _projetoService = projetoService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("cadastro")]
        [SwaggerOperation(Summary = "Cadastra um novo projeto", Description = "Requer autenticação com role Admin. Registra um novo projeto no sistema.")]
        [ProducesResponseType(typeof(ProjetosDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Inserir([FromBody] ProjetoNovoDto model)
        {
            return new JsonResult(await _projetoService.Inserir(model));
        }

        [Authorize]
        [HttpGet("buscartodos")]
        [SwaggerOperation(Summary = "Lista todos os projetos", Description = "Retorna todos os projetos cadastrados. Requer autenticação.")]
        [ProducesResponseType(typeof(IEnumerable<ProjetosDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Buscar() {
            var projetos = await _projetoService.BuscarTodos();
            return Ok(projetos);
        }

        [Authorize]
        [HttpGet("buscarfiltrado")]
        [SwaggerOperation(Summary = "Busca projetos com filtro", Description = "Permite buscar projetos por nome e/ou status. Requer autenticação.")]
        [ProducesResponseType(typeof(IEnumerable<ProjetosDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BuscarPorFiltro([FromQuery] string? nome, [FromQuery] StatusProjeto? status)
        {
            var projetos = await _projetoService.BuscarPorFiltro(nome, status);
            return Ok(projetos);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/deletar")]
        [SwaggerOperation(Summary = "Exclui um projeto", Description = "Remove um projeto com base no ID. Apenas Admins podem excluir. Projetos em execução ou finalizados não podem ser excluídos.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Deletar(long id)
        {
            try
            {
                await _projetoService.ExcluirProjetoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        [SwaggerOperation(Summary = "Atualiza o status de um projeto", Description = "Altera o status de um projeto existente. Acesso restrito a Admin.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizaStatusDTo dto)
        {
            try
            {
                await _projetoService.AtualizarProjetoAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
