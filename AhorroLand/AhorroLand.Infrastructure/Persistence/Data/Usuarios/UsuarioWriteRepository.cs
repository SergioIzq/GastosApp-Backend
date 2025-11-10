using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Usuarios
{

    public class UsuarioWriteRepository : AbsWriteRepository<Usuario>, IUsuarioWriteRepository
    {
        public UsuarioWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
