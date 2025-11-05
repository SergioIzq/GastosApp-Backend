using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.IngresosProgramados
{
    public class IngresoProgramadoReadRepository : AbsReadRepository<IngresoProgramado, IngresoProgramadoDto>, IIngresoProgramadoReadRepository
    {
        public IngresoProgramadoReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "IngresosProgramados")
        {
        }

    }
}