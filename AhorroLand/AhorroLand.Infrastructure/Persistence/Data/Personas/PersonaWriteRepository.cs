using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Personas
{
    public class PersonaWriteRepository : AbsWriteRepository<Persona>, IPersonaWriteRepository
    {
        private readonly IPersonaReadRepository _readRepository;

        public PersonaWriteRepository(
            AhorroLandDbContext context,
            IPersonaReadRepository readRepository) : base(context)
        {
            _readRepository = readRepository;
        }

        public override async Task CreateAsync(Persona entity, CancellationToken cancellationToken = default)
        {
            var exists = await _readRepository.ExistsWithSameNameAsync(
                entity.Nombre,
                entity.UsuarioId,
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Ya existe una persona con el nombre '{entity.Nombre.Value}' para este usuario.");
            }

            await base.CreateAsync(entity, cancellationToken);
        }

        public override void Update(Persona entity)
        {
            throw new NotSupportedException(
                "Use UpdateAsync para actualizar personas con validación de duplicados.");
        }

        public override async Task UpdateAsync(Persona entity, CancellationToken cancellationToken = default)
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
                    $"Ya existe otra persona con el nombre '{entity.Nombre.Value}' para este usuario.");
            }
        }
    }
}
