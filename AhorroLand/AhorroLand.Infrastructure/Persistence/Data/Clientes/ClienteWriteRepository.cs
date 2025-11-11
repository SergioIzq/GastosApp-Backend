using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Clientes
{
    public class ClienteWriteRepository : AbsWriteRepository<Cliente>, IClienteWriteRepository
    {
        private readonly IClienteReadRepository _readRepository;

        public ClienteWriteRepository(
            AhorroLandDbContext context,
            IClienteReadRepository readRepository) : base(context)
        {
            _readRepository = readRepository;
        }

        public override async Task CreateAsync(Cliente entity, CancellationToken cancellationToken = default)
        {
            var exists = await _readRepository.ExistsWithSameNameAsync(
                entity.Nombre,
                entity.UsuarioId,
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Ya existe un cliente con el nombre '{entity.Nombre.Value}' para este usuario.");
            }

            await base.CreateAsync(entity, cancellationToken);
        }

        public override void Update(Cliente entity)
        {
            throw new NotSupportedException(
                "Use UpdateAsync para actualizar clientes con validación de duplicados.");
        }

        public override async Task UpdateAsync(Cliente entity, CancellationToken cancellationToken = default)
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
                    $"Ya existe otro cliente con el nombre '{entity.Nombre.Value}' para este usuario.");
            }
        }
    }
}
