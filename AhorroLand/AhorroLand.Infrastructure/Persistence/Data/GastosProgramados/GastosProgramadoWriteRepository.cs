using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.GastosProgramados
{

    // Nota: Asegúrate de que IGastoProgramadoWriteRepository herede de IWriteRepository<GastoProgramado>
    public class GastoProgramadoWriteRepository : AbsWriteRepository<GastoProgramado>, IWriteRepository<GastoProgramado>
    {
        public GastoProgramadoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
