using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Cuentas;

public class CuentaReadRepository : AbsReadRepository<Cuenta, CuentaDto>, ICuentaReadRepository
{
    public CuentaReadRepository(IDbConnectionFactory dbConnectionFactory)
        : base(dbConnectionFactory, "Cuentas")
    {
    }

}