using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Categorias
{
    public class CategoriaWriteRepository : AbsWriteRepository<Categoria>, ICategoriaWriteRepository
    {
        private readonly ICategoriaReadRepository _readRepository;

        public CategoriaWriteRepository(
            AhorroLandDbContext context,
            ICategoriaReadRepository readRepository) : base(context)
        {
            _readRepository = readRepository;
        }

        public override async Task CreateAsync(Categoria entity, CancellationToken cancellationToken = default)
        {
            // Validar que no exista una categoría con el mismo nombre para el mismo usuario
            var exists = await _readRepository.ExistsWithSameNameAsync(
                entity.Nombre,
                entity.IdUsuario,
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Ya existe una categoría con el nombre '{entity.Nombre.Value}' para este usuario.");
            }

            await base.CreateAsync(entity, cancellationToken);
        }

        public override void Update(Categoria entity)
        {
            // Para updates síncronos, lanzamos una excepción indicando que se debe usar el método asíncrono
            throw new NotSupportedException(
                "Use UpdateAsync para actualizar categorías con validación de duplicados.");
        }

        /// <summary>
        /// Actualiza una categoría validando que no exista duplicado.
        /// </summary>
        public override async Task UpdateAsync(Categoria entity, CancellationToken cancellationToken = default)
        {
            // Primero verificar que la entidad existe (validación del base)
            await base.UpdateAsync(entity, cancellationToken);

            // Luego validar duplicados
            var exists = await _readRepository.ExistsWithSameNameExceptAsync(
                entity.Nombre,
                entity.IdUsuario,
                entity.Id,
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Ya existe otra categoría con el nombre '{entity.Nombre.Value}' para este usuario.");
            }
        }
    }
}
