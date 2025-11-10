using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.IngresosProgramados.Commands;

public sealed record DeleteIngresoProgramadoCommand(Guid Id) : AbsDeleteCommand<IngresoProgramado>(Id);
