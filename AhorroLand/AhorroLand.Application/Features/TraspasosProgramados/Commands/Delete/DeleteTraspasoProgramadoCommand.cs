using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.TraspasosProgramados.Commands;

public sealed record DeleteTraspasoProgramadoCommand(Guid Id) : AbsDeleteCommand<TraspasoProgramado>(Id);
