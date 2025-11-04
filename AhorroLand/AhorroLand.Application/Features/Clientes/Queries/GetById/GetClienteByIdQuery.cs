using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Categorias.Commands;

/// <summary>
/// Representa la solicitud para crear un nuevo Cliente.
/// </summary>
// Hereda de AbsCreateCommand<Entidad, DTO de Respuesta>
public sealed record GetClienteByIdQuery(Guid Id) : AbsGetByIdQuery<Cliente, ClienteDto>(Id)
{
}