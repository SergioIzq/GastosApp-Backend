using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.Categorias
{

    // Nota: Asegúrate de que ICategoriaWriteRepository herede de IWriteRepository<Categoria>
    public class CategoriaWriteRepository : AbsWriteRepository<Categoria>, IWriteRepository<Categoria>
    {
        public CategoriaWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
