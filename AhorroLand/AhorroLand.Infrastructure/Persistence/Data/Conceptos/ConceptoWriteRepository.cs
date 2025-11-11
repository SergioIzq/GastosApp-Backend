using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Conceptos
{
    public class ConceptoWriteRepository : AbsWriteRepository<Concepto>, IConceptoWriteRepository
    {
        private readonly IConceptoReadRepository _readRepository;

        public ConceptoWriteRepository(
          AhorroLandDbContext context,
          IConceptoReadRepository readRepository) : base(context)
        {
            _readRepository = readRepository;
        }

        public override async Task CreateAsync(Concepto entity, CancellationToken cancellationToken = default)
        {
            var exists = await _readRepository.ExistsWithSameNameAsync(
          entity.Nombre,
          entity.UsuarioId,
          cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                $"Ya existe un concepto con el nombre '{entity.Nombre.Value}' para este usuario.");
            }

            await base.CreateAsync(entity, cancellationToken);
        }

        public override void Update(Concepto entity)
        {
            throw new NotSupportedException(
          "Use UpdateAsync para actualizar conceptos con validación de duplicados.");
        }

        public override async Task UpdateAsync(Concepto entity, CancellationToken cancellationToken = default)
        {
            await base.UpdateAsync(entity, cancellationToken);

            var exists = await _readRepository.ExistsWithSameNameExceptAsync(
 entity.Nombre,
 entity.UsuarioId,
 entity.Id,
 cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
  $"Ya existe otro concepto con el nombre '{entity.Nombre.Value}' para este usuario.");
            }
        }
    }
}
