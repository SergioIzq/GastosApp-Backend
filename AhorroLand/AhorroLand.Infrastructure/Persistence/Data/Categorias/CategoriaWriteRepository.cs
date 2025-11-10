using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Categorias
{

    // Nota: Asegúrate de que ICategoriaWriteRepository herede de IWriteRepository<Categoria>
    public class CategoriaWriteRepository : AbsWriteRepository<Categoria>, ICategoriaWriteRepository
    {
        public CategoriaWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
