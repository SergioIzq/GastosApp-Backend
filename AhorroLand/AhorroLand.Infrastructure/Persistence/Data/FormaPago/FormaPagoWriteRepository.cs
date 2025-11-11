using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.FormasPago
{
    public class FormaPagoWriteRepository : AbsWriteRepository<FormaPago>, IFormaPagoWriteRepository
    {
        private readonly IFormaPagoReadRepository _readRepository;

        public FormaPagoWriteRepository(
            AhorroLandDbContext context,
            IFormaPagoReadRepository readRepository) : base(context)
        {
            _readRepository = readRepository;
        }

        public override async Task CreateAsync(FormaPago entity, CancellationToken cancellationToken = default)
        {
            var exists = await _readRepository.ExistsWithSameNameAsync(
                entity.Nombre,
                entity.UsuarioId,
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Ya existe una forma de pago con el nombre '{entity.Nombre.Value}' para este usuario.");
            }

            await base.CreateAsync(entity, cancellationToken);
        }

        public override void Update(FormaPago entity)
        {
            throw new NotSupportedException(
                "Use UpdateAsync para actualizar formas de pago con validación de duplicados.");
        }

        public override async Task UpdateAsync(FormaPago entity, CancellationToken cancellationToken = default)
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
                    $"Ya existe otra forma de pago con el nombre '{entity.Nombre.Value}' para este usuario.");
            }
        }
    }
}
