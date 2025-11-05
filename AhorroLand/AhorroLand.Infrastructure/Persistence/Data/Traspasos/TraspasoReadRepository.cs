using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Traspasos
{
    public class TraspasoReadRepository : AbsReadRepository<Traspaso, TraspasoDto>, ITraspasoReadRepository
    {
        public TraspasoReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "Traspasos")
        {
        }

    }
}