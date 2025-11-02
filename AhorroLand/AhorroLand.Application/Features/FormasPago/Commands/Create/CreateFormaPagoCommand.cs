using AhorroLand.Domain.FormasPago;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.FormasPago.Commands;

public sealed record CreateFormaPagoCommand : AbsCreateCommand<FormaPago, FormaPagoDto>
{
 public required string Nombre { get; init; }
}
