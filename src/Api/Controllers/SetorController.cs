using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTickets.Api.Responses;
using SupportTickets.Application.DTOs.Setor;
using SupportTickets.Application.Interfaces;

namespace SupportTickets.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/setores")]
public sealed class SetorController : ControllerBase
{
    private readonly ISetorService _setorService;

    public SetorController(ISetorService setorService)
    {
        _setorService = setorService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SetorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _setorService.ListarAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<SetorResponse>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SetorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _setorService.ObterPorIdAsync(id, cancellationToken);
        return Ok(ApiResponse<SetorResponse>.Ok(result));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SetorResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] SetorRequest request, CancellationToken cancellationToken)
    {
        var result = await _setorService.CriarAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<SetorResponse>.Ok(result, "Setor criado com sucesso."));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SetorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] SetorRequest request, CancellationToken cancellationToken)
    {
        var result = await _setorService.AtualizarAsync(id, request, cancellationToken);
        return Ok(ApiResponse<SetorResponse>.Ok(result, "Setor atualizado com sucesso."));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _setorService.RemoverAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(null, "Setor removido com sucesso."));
    }
}
