using AhorroLand.Shared.Application.Abstractions.Servicies;

namespace AhorroLand.Infrastructure.Services.Scheduling;

/// <summary>
/// Implementación del servicio de generación de Job IDs para tareas programadas.
/// Utiliza Guid para garantizar unicidad en entornos distribuidos.
/// </summary>
public sealed class JobSchedulingService : IJobSchedulingService
{
    /// <summary>
    /// Genera un identificador único para un trabajo programado.
    /// </summary>
    /// <returns>Un string único basado en GUID.</returns>
    public string GenerateJobId()
    {
        return Guid.NewGuid().ToString();
    }
}
