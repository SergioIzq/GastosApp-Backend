using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.GastosProgramados.Commands;

public sealed record DeleteGastoProgramadoCommand(Guid Id) : AbsDeleteCommand<GastoProgramado>(Id);
