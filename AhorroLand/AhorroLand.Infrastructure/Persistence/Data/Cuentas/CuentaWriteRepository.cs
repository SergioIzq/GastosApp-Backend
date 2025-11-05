using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.Cuentas
{

    // Nota: Asegúrate de que ICuentaWriteRepository herede de IWriteRepository<Cuenta>
    public class CuentaWriteRepository : AbsWriteRepository<Cuenta>, IWriteRepository<Cuenta>
    {
        public CuentaWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
