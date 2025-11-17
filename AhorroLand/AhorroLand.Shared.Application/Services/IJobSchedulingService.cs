namespace AhorroLand.Shared.Application.Abstractions.Servicies;

/// <summary>
/// Servicio de aplicación para la generación de identificadores de trabajos programados.
/// Este servicio abstrae la lógica de generación de IDs para tareas en segundo plano (Hangfire, Quartz, etc.).
/// </summary>
public interface IJobSchedulingService
{
    /// <summary>
    /// Genera un identificador único para un trabajo programado.
    /// </summary>
    /// <returns>Un string único que representa el Job ID.</returns>
    string GenerateJobId();
}
