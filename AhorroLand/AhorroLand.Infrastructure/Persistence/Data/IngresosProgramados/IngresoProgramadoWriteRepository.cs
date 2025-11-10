using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.IngresosProgramados
{

    // Nota: Asegúrate de que IIngresoProgramadoWriteRepository herede de IWriteRepository<IngresoProgramado>
    public class IngresoProgramadoWriteRepository : AbsWriteRepository<IngresoProgramado>, IIngresoProgramadoWriteRepository
    {
        public IngresoProgramadoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
