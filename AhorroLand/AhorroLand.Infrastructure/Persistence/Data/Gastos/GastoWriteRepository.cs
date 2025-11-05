using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.Gastos
{

    // Nota: Asegúrate de que IGastoWriteRepository herede de IWriteRepository<Gasto>
    public class GastoWriteRepository : AbsWriteRepository<Gasto>, IWriteRepository<Gasto>
    {
        public GastoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
