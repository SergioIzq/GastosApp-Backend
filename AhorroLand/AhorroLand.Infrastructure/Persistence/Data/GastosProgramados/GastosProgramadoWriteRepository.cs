using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.GastosProgramados
{

    // Nota: Asegúrate de que IGastoProgramadoWriteRepository herede de IWriteRepository<GastoProgramado>
    public class GastoProgramadoWriteRepository : AbsWriteRepository<GastoProgramado>, IGastoProgramadoWriteRepository
    {
        public GastoProgramadoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
