using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Proveedores
{

    // Nota: Asegúrate de que IProveedorWriteRepository herede de IWriteRepository<Proveedor>
    public class ProveedorWriteRepository : AbsWriteRepository<Proveedor>, IProveedorWriteRepository
    {
        public ProveedorWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
