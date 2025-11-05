using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.Usuarios
{

    // Nota: Asegúrate de que IUsuarioWriteRepository herede de IWriteRepository<Usuario>
    public class UsuarioWriteRepository : AbsWriteRepository<Usuario>, IWriteRepository<Usuario>
    {
        public UsuarioWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
