using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Query;

/// <summary>
/// Repositorio especializado para consultas del dashboard.
/// </summary>
public interface IDashboardRepository
{
    /// <summary>
    /// Obtiene el resumen completo del dashboard para un usuario con filtros opcionales.
    /// </summary>
    Task<DashboardResumenDto?> GetDashboardResumenAsync(
        Guid usuarioId,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        Guid? cuentaId = null,
        Guid? categoriaId = null,
        CancellationToken cancellationToken = default);
}
