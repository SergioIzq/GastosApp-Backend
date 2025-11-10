using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.TraspasosProgramados.Queries;

public sealed record GetTraspasoProgramadoByIdQuery(Guid Id) : AbsGetByIdQuery<TraspasoProgramado, TraspasoProgramadoDto>(Id)
{
}
