using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Proveedores
{
    public class ProveedorReadRepository : AbsReadRepository<Proveedor, ProveedorDto>, IProveedorReadRepository
    {
        public ProveedorReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "Proveedores")
        {
        }

    }
}