using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.Proveedores
{

    // Nota: Asegúrate de que IProveedorWriteRepository herede de IWriteRepository<Proveedor>
    public class ProveedorWriteRepository : AbsWriteRepository<Proveedor>, IWriteRepository<Proveedor>
    {
        public ProveedorWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
