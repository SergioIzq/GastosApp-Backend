using AhorroLand.Application.Interfaces;
using AhorroLand.Shared.Application.Abstractions.Messaging;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Abstractions.Results;

namespace AhorroLand.Application.Features.Dashboard.Queries;

/// <summary>
/// Handler para obtener el resumen del dashboard con filtros opcionales.
/// </summary>
public sealed class GetDashboardResumenQueryHandler : IQueryHandler<GetDashboardResumenQuery, DashboardResumenDto>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetDashboardResumenQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<Result<DashboardResumenDto>> Handle(GetDashboardResumenQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener el resumen completo del dashboard con filtros
            var resumen = await _dashboardRepository.GetDashboardResumenAsync(
          request.UsuarioId,
       request.FechaInicio,
                  request.FechaFin,
             request.CuentaId,
       request.CategoriaId,
          cancellationToken);

            if (resumen == null)
            {
                return Result.Failure<DashboardResumenDto>(
              Error.NotFound("No se pudo generar el resumen del dashboard."));
            }

            return Result.Success(resumen);
        }
        catch (Exception ex)
        {
            return Result.Failure<DashboardResumenDto>(
                Error.Failure("Dashboard.Error", "Error al obtener resumen", ex.Message));
        }
    }
}
