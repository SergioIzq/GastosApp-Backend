using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.TraspasosProgramados
{

    // Nota: Asegúrate de que ITraspasoProgramadoWriteRepository herede de IWriteRepository<TraspasoProgramado>
    public class TraspasoProgramadoWriteRepository : AbsWriteRepository<TraspasoProgramado>, IWriteRepository<TraspasoProgramado>
    {
        public TraspasoProgramadoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
