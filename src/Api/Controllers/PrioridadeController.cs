using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTickets.Api.Responses;
using SupportTickets.Application.DTOs.Prioridade;
using SupportTickets.Application.Interfaces;

namespace SupportTickets.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/prioridades")]
public sealed class PrioridadeController : ControllerBase
{
    private readonly IPrioridadeService _prioridadeService;

    public PrioridadeController(IPrioridadeService prioridadeService)
    {
        _prioridadeService = prioridadeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PrioridadeResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _prioridadeService.ListarAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<PrioridadeResponse>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<PrioridadeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _prioridadeService.ObterPorIdAsync(id, cancellationToken);
        return Ok(ApiResponse<PrioridadeResponse>.Ok(result));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PrioridadeResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] PrioridadeRequest request, CancellationToken cancellationToken)
    {
        var result = await _prioridadeService.CriarAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<PrioridadeResponse>.Ok(result, "Prioridade criada com sucesso."));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<PrioridadeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] PrioridadeRequest request, CancellationToken cancellationToken)
    {
        var result = await _prioridadeService.AtualizarAsync(id, request, cancellationToken);
        return Ok(ApiResponse<PrioridadeResponse>.Ok(result, "Prioridade atualizada com sucesso."));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _prioridadeService.RemoverAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(null, "Prioridade removida com sucesso."));
    }
}
