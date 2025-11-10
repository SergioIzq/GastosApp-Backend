using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.GastosProgramados.Queries;

public sealed record GetGastoProgramadoByIdQuery(Guid Id) : AbsGetByIdQuery<GastoProgramado, GastoProgramadoDto>(Id)
{
}
