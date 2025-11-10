using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Traspasos.Queries;

public sealed record GetTraspasoByIdQuery(Guid Id) : AbsGetByIdQuery<Traspaso, TraspasoDto>(Id)
{
}
