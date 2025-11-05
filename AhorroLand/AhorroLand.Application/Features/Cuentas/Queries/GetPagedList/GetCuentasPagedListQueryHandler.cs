using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AhorroLand.Application.Features.Cuentas.Queries;

/// <summary>
/// Manejador concreto para la consulta de lista paginada de Categorías.
/// Implementa la lógica específica de filtrado y ordenación.
/// </summary>
public sealed class GetCuentasPagedListQueryHandler
    : GetPagedListQueryHandler<Cuenta, CuentaDto, GetCuentasPagedListQuery>
{
    public GetCuentasPagedListQueryHandler(
        IReadRepository<Cuenta> repository,
        ICacheService cacheService)
        : base(repository, cacheService)
    {
        // No se necesita lógica adicional en el constructor.
    }    
}