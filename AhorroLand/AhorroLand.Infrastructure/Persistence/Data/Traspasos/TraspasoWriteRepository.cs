using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.Traspasos
{

    // Nota: Asegúrate de que ITraspasoWriteRepository herede de IWriteRepository<Traspaso>
    public class TraspasoWriteRepository : AbsWriteRepository<Traspaso>, IWriteRepository<Traspaso>
    {
        public TraspasoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
