using SupportTickets.Application.DTOs.Chamado;
using SupportTickets.Application.Interfaces;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Application.Services;

public sealed class ChamadoService : IChamadoService
{
    private readonly IChamadoRepository _chamadoRepository;
    private readonly ISetorRepository _setorRepository;
    private readonly IPrioridadeRepository _prioridadeRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ChamadoService(
        IChamadoRepository chamadoRepository,
        ISetorRepository setorRepository,
        IPrioridadeRepository prioridadeRepository,
        IUsuarioRepository usuarioRepository)
    {
        _chamadoRepository = chamadoRepository;
        _setorRepository = setorRepository;
        _prioridadeRepository = prioridadeRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ChamadoResponse> CriarAsync(CriarChamadoRequest request, int usuarioId, CancellationToken cancellationToken = default)
    {
        await GarantirUsuarioAtivoAsync(usuarioId, cancellationToken);

        if (!await _setorRepository.ExistsAsync(request.SetorId, cancellationToken))
        {
            throw new NotFoundException("O setor informado para o chamado não foi encontrado.");
        }

        if (!await _prioridadeRepository.ExistsAsync(request.PrioridadeId, cancellationToken))
        {
            throw new NotFoundException("A prioridade informada para o chamado não foi encontrada.");
        }

        var chamado = Chamado.Criar(
            request.Titulo,
            request.Descricao,
            usuarioId,
            request.SetorId,
            request.PrioridadeId,
            DateTime.UtcNow);

        await _chamadoRepository.AddAsync(chamado, cancellationToken);
        await _chamadoRepository.SaveChangesAsync(cancellationToken);

        var chamadoCompleto = await _chamadoRepository.GetByIdComDetalhesAsync(chamado.Id, cancellationToken)
                             ?? throw new NotFoundException("Chamado não encontrado após a criação.");

        return MapDetalhado(chamadoCompleto);
    }

    public async Task<ChamadoResponse> ObterPorIdAsync(
        int id,
        int usuarioId,
        PerfilUsuario perfil,
        CancellationToken cancellationToken = default)
    {
        var chamado = await _chamadoRepository.GetByIdComDetalhesAsync(id, cancellationToken)
                     ?? throw new NotFoundException("Chamado não encontrado.");

        GarantirPermissaoLeitura(chamado, usuarioId, perfil);

        return MapDetalhado(chamado);
    }

    public async Task<IReadOnlyList<ChamadoListItemResponse>> ListarAsync(
        ChamadoFiltroRequest filtro,
        int usuarioId,
        PerfilUsuario perfil,
        CancellationToken cancellationToken = default)
    {
        var chamados = await _chamadoRepository.ListarComDetalhesAsync(filtro, perfil, usuarioId, cancellationToken);
        var agora = DateTime.UtcNow;

        return chamados
            .Select(chamado => MapListItem(chamado, agora))
            .Where(item => !filtro.ApenasAtrasados || item.EstaAtrasado)
            .ToList();
    }

    public async Task<ChamadoResponse> IniciarAsync(int id, int atendenteId, CancellationToken cancellationToken = default)
    {
        await GarantirUsuarioAtivoAsync(atendenteId, cancellationToken);

        var chamado = await _chamadoRepository.GetByIdComDetalhesAsync(id, cancellationToken)
                     ?? throw new NotFoundException("Chamado não encontrado.");

        chamado.IniciarAtendimento(atendenteId, DateTime.UtcNow);

        _chamadoRepository.Update(chamado);
        await _chamadoRepository.SaveChangesAsync(cancellationToken);

        return MapDetalhado(chamado);
    }

    public async Task<ChamadoResponse> FinalizarAsync(
        int id,
        FinalizarChamadoRequest request,
        int usuarioId,
        PerfilUsuario perfil,
        CancellationToken cancellationToken = default)
    {
        await GarantirUsuarioAtivoAsync(usuarioId, cancellationToken);

        var chamado = await _chamadoRepository.GetByIdComDetalhesAsync(id, cancellationToken)
                     ?? throw new NotFoundException("Chamado não encontrado.");

        if (perfil == PerfilUsuario.Atendente && !chamado.AtendimentoPertenceAo(usuarioId))
        {
            throw new ForbiddenException("Somente o atendente responsável pode finalizar este chamado.");
        }

        chamado.Finalizar(usuarioId, request.Solucao, DateTime.UtcNow);

        _chamadoRepository.Update(chamado);
        await _chamadoRepository.SaveChangesAsync(cancellationToken);

        return MapDetalhado(chamado);
    }

    public async Task<ChamadoResponse> CancelarAsync(
        int id,
        int usuarioId,
        PerfilUsuario perfil,
        CancellationToken cancellationToken = default)
    {
        await GarantirUsuarioAtivoAsync(usuarioId, cancellationToken);

        var chamado = await _chamadoRepository.GetByIdComDetalhesAsync(id, cancellationToken)
                     ?? throw new NotFoundException("Chamado não encontrado.");

        if (perfil == PerfilUsuario.Usuario && !chamado.FoiAbertoPor(usuarioId))
        {
            throw new ForbiddenException("Você só pode cancelar chamados que foram abertos por você.");
        }

        if (perfil == PerfilUsuario.Atendente &&
            !chamado.FoiAbertoPor(usuarioId) &&
            !chamado.AtendimentoPertenceAo(usuarioId))
        {
            throw new ForbiddenException("Você só pode cancelar chamados que abriu ou que está atendendo.");
        }

        chamado.Cancelar(usuarioId, DateTime.UtcNow);

        _chamadoRepository.Update(chamado);
        await _chamadoRepository.SaveChangesAsync(cancellationToken);

        return MapDetalhado(chamado);
    }

    public async Task<ChamadoResponse> AlterarPrioridadeAsync(
        int id,
        AtualizarPrioridadeChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _prioridadeRepository.ExistsAsync(request.PrioridadeId, cancellationToken))
        {
            throw new NotFoundException("A nova prioridade informada não foi encontrada.");
        }

        var chamado = await _chamadoRepository.GetByIdComDetalhesAsync(id, cancellationToken)
                     ?? throw new NotFoundException("Chamado não encontrado.");

        chamado.AlterarPrioridade(request.PrioridadeId);

        _chamadoRepository.Update(chamado);
        await _chamadoRepository.SaveChangesAsync(cancellationToken);

        var chamadoAtualizado = await _chamadoRepository.GetByIdComDetalhesAsync(id, cancellationToken)
                               ?? chamado;

        return MapDetalhado(chamadoAtualizado);
    }

    private async Task GarantirUsuarioAtivoAsync(int usuarioId, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId, cancellationToken)
                     ?? throw new UnauthorizedException("Usuário autenticado não encontrado.");

        if (!usuario.Ativo)
        {
            throw new UnauthorizedException("Usuário autenticado está inativo.");
        }
    }

    private static void GarantirPermissaoLeitura(Chamado chamado, int usuarioId, PerfilUsuario perfil)
    {
        if (perfil == PerfilUsuario.Usuario && !chamado.FoiAbertoPor(usuarioId))
        {
            throw new ForbiddenException("Você não tem permissão para visualizar este chamado.");
        }
    }

    private static ChamadoResponse MapDetalhado(Chamado chamado)
    {
        var agora = DateTime.UtcNow;
        var tempoTotal = chamado.ObterTempoTotalAtendimento(agora);
        var sla = chamado.Prioridade?.TempoEstimadoHoras ?? 0;

        return new ChamadoResponse
        {
            Id = chamado.Id,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            UsuarioId = chamado.UsuarioId,
            Solicitante = chamado.Usuario?.Nome ?? string.Empty,
            SetorId = chamado.SetorId,
            Setor = chamado.Setor?.Nome ?? string.Empty,
            PrioridadeId = chamado.PrioridadeId,
            Prioridade = chamado.Prioridade?.Nome ?? string.Empty,
            SlaHoras = sla,
            Status = chamado.Status,
            CriadoEm = chamado.CriadoEm,
            IniciadoEm = chamado.Atendimento?.IniciadoEm,
            AtendenteId = chamado.Atendimento?.AtendenteId,
            Atendente = chamado.Atendimento?.Atendente?.Nome,
            FinalizadoEm = chamado.FinalizadoEm,
            FinalizadoPor = chamado.FinalizadoPor,
            CanceladoEm = chamado.CanceladoEm,
            CanceladoPor = chamado.CanceladoPor,
            Solucao = chamado.Atendimento?.Solucao,
            TempoTotalAtendimento = tempoTotal.ToString(@"d\.hh\:mm\:ss"),
            TempoTotalAtendimentoHoras = Math.Round(tempoTotal.TotalHours, 2),
            EstaAtrasado = tempoTotal > TimeSpan.FromHours(sla),
            HistoricoStatus = chamado.Historicos
                .OrderBy(x => x.AlteradoEm)
                .Select(x => new HistoricoStatusResponse
                {
                    Id = x.Id,
                    Status = x.Status,
                    AlteradoPor = x.AlteradoPor,
                    AlteradoEm = x.AlteradoEm
                })
                .ToList()
        };
    }

    private static ChamadoListItemResponse MapListItem(Chamado chamado, DateTime referenciaUtc)
    {
        var tempoTotal = chamado.ObterTempoTotalAtendimento(referenciaUtc);
        var sla = chamado.Prioridade?.TempoEstimadoHoras ?? 0;

        return new ChamadoListItemResponse
        {
            Id = chamado.Id,
            Titulo = chamado.Titulo,
            Setor = chamado.Setor?.Nome ?? string.Empty,
            Prioridade = chamado.Prioridade?.Nome ?? string.Empty,
            Status = chamado.Status,
            CriadoEm = chamado.CriadoEm,
            IniciadoEm = chamado.Atendimento?.IniciadoEm,
            FinalizadoEm = chamado.FinalizadoEm,
            Solicitante = chamado.Usuario?.Nome ?? string.Empty,
            Atendente = chamado.Atendimento?.Atendente?.Nome,
            TempoTotalAtendimento = tempoTotal.ToString(@"d\.hh\:mm\:ss"),
            TempoTotalAtendimentoHoras = Math.Round(tempoTotal.TotalHours, 2),
            SlaHoras = sla,
            EstaAtrasado = tempoTotal > TimeSpan.FromHours(sla)
        };
    }
}
