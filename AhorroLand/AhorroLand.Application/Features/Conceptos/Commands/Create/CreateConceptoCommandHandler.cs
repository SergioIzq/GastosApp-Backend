using AhorroLand.Domain.Categorias;
using AhorroLand.Domain.Conceptos;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.Conceptos.Commands;

public sealed class CreateConceptoCommandHandler : AbsCreateCommandHandler<Concepto, ConceptoDto, CreateConceptoCommand>
{
    private readonly IReadOnlyRepository<Categoria> _categoriaRepository;

    public CreateConceptoCommandHandler(
    IUnitOfWork unitOfWork,
    IWriteRepository<Concepto> writeRepository,
    ICacheService cacheService,
    IReadOnlyRepository<Categoria> categoriaRepository)
    : base(unitOfWork, writeRepository, cacheService)
    {
        _categoriaRepository = categoriaRepository;
    }

    protected override Concepto CreateEntity(CreateConceptoCommand command)
    {
        var nombreVO = new Nombre(command.Nombre);

        var categoria = _categoriaRepository.GetByIdAsync(command.CategoriaId).ConfigureAwait(false).GetAwaiter().GetResult();
        if (categoria is null)
            throw new InvalidOperationException($"Categoria con id {command.CategoriaId} no encontrada.");

        var newConcepto = Concepto.Create(Guid.NewGuid(), nombreVO, categoria);

        return newConcepto;
    }
}
