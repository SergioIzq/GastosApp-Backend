using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.Clientes
{

    // Nota: Asegúrate de que IClienteWriteRepository herede de IWriteRepository<Cliente>
    public class ClienteWriteRepository : AbsWriteRepository<Cliente>, IWriteRepository<Cliente>
    {
        public ClienteWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
