using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Gastos
{

    // Nota: Asegúrate de que IGastoWriteRepository herede de IWriteRepository<Gasto>
    public class GastoWriteRepository : AbsWriteRepository<Gasto>, IGastoWriteRepository
    {
        public GastoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
