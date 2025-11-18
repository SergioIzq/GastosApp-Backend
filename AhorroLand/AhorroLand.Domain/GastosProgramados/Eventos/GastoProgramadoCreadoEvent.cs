using AhorroLand.Shared.Domain.Events;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.GastosProgramados.Eventos;

/// <summary>
/// Evento de dominio que se dispara cuando se crea un nuevo GastoProgramado.
/// Este evento es escuchado por la infraestructura para programar el job en Hangfire.
/// </summary>
public sealed record GastoProgramadoCreadoEvent(
    Guid GastoProgramadoId,
    Frecuencia Frecuencia,
    DateTime FechaEjecucion
) : DomainEventBase;
