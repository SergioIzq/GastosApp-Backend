using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Traspasos
{

    // Nota: Asegúrate de que ITraspasoWriteRepository herede de IWriteRepository<Traspaso>
    public class TraspasoWriteRepository : AbsWriteRepository<Traspaso>, ITraspasoWriteRepository
    {
        public TraspasoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
