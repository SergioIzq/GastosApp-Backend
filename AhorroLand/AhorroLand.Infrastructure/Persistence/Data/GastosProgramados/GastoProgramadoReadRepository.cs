using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.GastosProgramados
{
    public class GastoProgramadoReadRepository : AbsReadRepository<GastoProgramado, GastoProgramadoDto>, IGastoProgramadoReadRepository
    {
        public GastoProgramadoReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "gastosProgramados")
        {
        }

    }
}