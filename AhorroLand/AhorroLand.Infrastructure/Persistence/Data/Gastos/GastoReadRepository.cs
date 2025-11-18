using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Gastos
{
    public class GastoReadRepository : AbsReadRepository<Gasto, GastoDto>, IGastoReadRepository
    {
        public GastoReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "gastos")
        {
        }

    }
}