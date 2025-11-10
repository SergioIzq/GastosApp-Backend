using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Ingresos
{

    // Nota: Asegúrate de que IIngresoWriteRepository herede de IWriteRepository<Ingreso>
    public class IngresoWriteRepository : AbsWriteRepository<Ingreso>, IIngresoWriteRepository
    {
        public IngresoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
