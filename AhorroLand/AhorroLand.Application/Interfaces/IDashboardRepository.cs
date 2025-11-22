using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Interfaces;

/// <summary>
/// Interfaz para el repositorio de Dashboard en la capa de Application.
/// </summary>
public interface IDashboardRepository
{
    /// <summary>
    /// Obtiene el resumen completo del dashboard para un usuario con filtros opcionales.
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <param name="fechaInicio">Fecha de inicio del período (opcional, default: primer día del mes actual)</param>
    /// <param name="fechaFin">Fecha de fin del período (opcional, default: último día del mes actual)</param>
    /// <param name="cuentaId">Filtrar por cuenta específica (opcional)</param>
    /// <param name="categoriaId">Filtrar por categoría específica (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task<DashboardResumenDto?> GetDashboardResumenAsync(
        Guid usuarioId,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        Guid? cuentaId = null,
        Guid? categoriaId = null,
        CancellationToken cancellationToken = default);
}
