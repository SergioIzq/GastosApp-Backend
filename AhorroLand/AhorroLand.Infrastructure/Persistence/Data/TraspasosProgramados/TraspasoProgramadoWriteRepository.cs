using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.TraspasosProgramados
{

    // Nota: Asegúrate de que ITraspasoProgramadoWriteRepository herede de IWriteRepository<TraspasoProgramado>
    public class TraspasoProgramadoWriteRepository : AbsWriteRepository<TraspasoProgramado>, ITraspasoProgramadoWriteRepository
    {
        public TraspasoProgramadoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
