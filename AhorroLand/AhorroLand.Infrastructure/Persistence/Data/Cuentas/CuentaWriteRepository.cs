using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Cuentas
{
    public class CuentaWriteRepository : AbsWriteRepository<Cuenta>, ICuentaWriteRepository
    {
        private readonly ICuentaReadRepository _readRepository;

        public CuentaWriteRepository(
            AhorroLandDbContext context,
            ICuentaReadRepository readRepository) : base(context)
        {
            _readRepository = readRepository;
        }

        public override async Task CreateAsync(Cuenta entity, CancellationToken cancellationToken = default)
        {
            var exists = await _readRepository.ExistsWithSameNameAsync(
                entity.Nombre,
                entity.UsuarioId,
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Ya existe una cuenta con el nombre '{entity.Nombre.Value}' para este usuario.");
            }

            await base.CreateAsync(entity, cancellationToken);
        }

        public override void Update(Cuenta entity)
        {
            throw new NotSupportedException(
                "Use UpdateAsync para actualizar cuentas con validación de duplicados.");
        }

        public override async Task UpdateAsync(Cuenta entity, CancellationToken cancellationToken = default)
        {
            // Primero verificar que la entidad existe (validación del base)
            await base.UpdateAsync(entity, cancellationToken);

            // Luego validar duplicados
            var exists = await _readRepository.ExistsWithSameNameExceptAsync(
                entity.Nombre,
                entity.UsuarioId,
                entity.Id,
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Ya existe otra cuenta con el nombre '{entity.Nombre.Value}' para este usuario.");
            }
        }
    }
}
