using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Proveedores
{

    // Nota: Asegúrate de que IProveedorWriteRepository herede de IWriteRepository<Proveedor>
    public class ProveedorWriteRepository : AbsWriteRepository<Proveedor>, IProveedorWriteRepository
    {
        private readonly IProveedorReadRepository _readRepository;

        public ProveedorWriteRepository(
      AhorroLandDbContext context,
     IProveedorReadRepository readRepository) : base(context)
        {
            _readRepository = readRepository;
        }

        public override async Task CreateAsync(Proveedor entity, CancellationToken cancellationToken = default)
        {
            var exists = await _readRepository.ExistsWithSameNameAsync(
      entity.Nombre,
    entity.UsuarioId,
     cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                 $"Ya existe un proveedor con el nombre '{entity.Nombre.Value}' para este usuario.");
            }

            await base.CreateAsync(entity, cancellationToken);
        }

        public override void Update(Proveedor entity)
        {
            throw new NotSupportedException(
             "Use UpdateAsync para actualizar proveedores con validación de duplicados.");
        }

        public override async Task UpdateAsync(Proveedor entity, CancellationToken cancellationToken = default)
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
              $"Ya existe otro proveedor con el nombre '{entity.Nombre.Value}' para este usuario.");
            }
        }
    }
}
