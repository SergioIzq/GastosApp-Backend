using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Clientes
{

    // Nota: Asegúrate de que IClienteWriteRepository herede de IWriteRepository<Cliente>
    public class ClienteWriteRepository : AbsWriteRepository<Cliente>, IClienteWriteRepository
    {
        public ClienteWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
