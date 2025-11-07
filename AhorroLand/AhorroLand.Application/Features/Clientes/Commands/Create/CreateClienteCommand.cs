using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Clientes.Commands;

/// <summary>
/// Representa la solicitud para crear una nueva Cliente.
/// </summary>
public sealed record CreateClienteCommand(string Nombre, Guid UsuarioId) : AbsCreateCommand<Cliente, ClienteDto>
{
}