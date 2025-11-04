using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.Categorias.Commands;

/// <summary>
/// Maneja la creación de una nueva entidad Categoria.
/// </summary>
public sealed class CreateCategoriaCommandHandler
    : AbsCreateCommandHandler<Categoria, CategoriaDto, CreateCategoriaCommand>
{
    public CreateCategoriaCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<Categoria> writeRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, cacheService)
    {
    }

    /// <summary>
    /// **Implementación de la lógica de negocio**: Crea la entidad Categoria.
    /// Este es el único método que tienes que implementar y donde se aplica el DDD.
    /// </summary>
    /// <param name="command">El comando con los datos de creación.</param>
    /// <returns>La nueva entidad Categoria creada.</returns>
    protected override Categoria CreateEntity(CreateCategoriaCommand command)
    {
        var nombreVO = new Nombre(command.Nombre);
        var descripcionVO = new Descripcion(command.Descripcion ?? string.Empty);
        var usuarioId = new UsuarioId(command.UsuarioId);

        var newCategoria = Categoria.Create(
            nombreVO,
            usuarioId,
            descripcionVO
        );

        return newCategoria;
    }
}