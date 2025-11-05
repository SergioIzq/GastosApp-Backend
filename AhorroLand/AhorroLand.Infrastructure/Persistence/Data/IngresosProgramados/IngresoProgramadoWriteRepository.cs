using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.IngresosProgramados
{

    // Nota: Asegúrate de que IIngresoProgramadoWriteRepository herede de IWriteRepository<IngresoProgramado>
    public class IngresoProgramadoWriteRepository : AbsWriteRepository<IngresoProgramado>, IWriteRepository<IngresoProgramado>
    {
        public IngresoProgramadoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
