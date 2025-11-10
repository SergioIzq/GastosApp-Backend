using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.Traspasos.Commands;

public sealed record DeleteTraspasoCommand(Guid Id) : AbsDeleteCommand<Traspaso>(Id);
