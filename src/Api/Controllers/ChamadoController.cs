using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTickets.Api.Responses;
using SupportTickets.Application.DTOs.Chamado;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/chamados")]
public sealed class ChamadoController : ControllerBase
{
    private readonly IChamadoService _chamadoService;

    public ChamadoController(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ChamadoListItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ChamadoFiltroRequest filtro, CancellationToken cancellationToken)
    {
        var result = await _chamadoService.ListarAsync(filtro, GetUsuarioId(), GetPerfilUsuario(), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ChamadoListItemResponse>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ChamadoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _chamadoService.ObterPorIdAsync(id, GetUsuarioId(), GetPerfilUsuario(), cancellationToken);
        return Ok(ApiResponse<ChamadoResponse>.Ok(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ChamadoResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CriarChamadoRequest request, CancellationToken cancellationToken)
    {
        var result = await _chamadoService.CriarAsync(request, GetUsuarioId(), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<ChamadoResponse>.Ok(result, "Chamado criado com sucesso."));
    }

    [Authorize(Roles = "Admin,Atendente")]
    [HttpPost("{id:int}/iniciar")]
    [ProducesResponseType(typeof(ApiResponse<ChamadoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Start(int id, CancellationToken cancellationToken)
    {
        var result = await _chamadoService.IniciarAsync(id, GetUsuarioId(), cancellationToken);
        return Ok(ApiResponse<ChamadoResponse>.Ok(result, "Atendimento iniciado com sucesso."));
    }

    [Authorize(Roles = "Admin,Atendente")]
    [HttpPost("{id:int}/finalizar")]
    [ProducesResponseType(typeof(ApiResponse<ChamadoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Finish(int id, [FromBody] FinalizarChamadoRequest request, CancellationToken cancellationToken)
    {
        var result = await _chamadoService.FinalizarAsync(id, request, GetUsuarioId(), GetPerfilUsuario(), cancellationToken);
        return Ok(ApiResponse<ChamadoResponse>.Ok(result, "Chamado finalizado com sucesso."));
    }

    [HttpPost("{id:int}/cancelar")]
    [ProducesResponseType(typeof(ApiResponse<ChamadoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var result = await _chamadoService.CancelarAsync(id, GetUsuarioId(), GetPerfilUsuario(), cancellationToken);
        return Ok(ApiResponse<ChamadoResponse>.Ok(result, "Chamado cancelado com sucesso."));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/prioridade")]
    [ProducesResponseType(typeof(ApiResponse<ChamadoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePriority(int id, [FromBody] AtualizarPrioridadeChamadoRequest request, CancellationToken cancellationToken)
    {
        var result = await _chamadoService.AlterarPrioridadeAsync(id, request, cancellationToken);
        return Ok(ApiResponse<ChamadoResponse>.Ok(result, "Prioridade do chamado atualizada com sucesso."));
    }

    private int GetUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(claim, out var usuarioId) || usuarioId <= 0)
        {
            throw new UnauthorizedException("Não foi possível identificar o usuário autenticado.");
        }

        return usuarioId;
    }

    private PerfilUsuario GetPerfilUsuario()
    {
        var claim = User.FindFirstValue(ClaimTypes.Role);

        if (!Enum.TryParse<PerfilUsuario>(claim, true, out var perfil))
        {
            throw new UnauthorizedException("Não foi possível identificar o perfil do usuário autenticado.");
        }

        return perfil;
    }
}
