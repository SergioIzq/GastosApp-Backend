using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Cuentas
{

    // Nota: Asegúrate de que ICuentaWriteRepository herede de IWriteRepository<Cuenta>
    public class CuentaWriteRepository : AbsWriteRepository<Cuenta>, ICuentaWriteRepository
    {
        public CuentaWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
