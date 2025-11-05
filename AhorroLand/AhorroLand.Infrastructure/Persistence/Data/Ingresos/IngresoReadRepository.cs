using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Ingresos
{
    public class IngresoReadRepository : AbsReadRepository<Ingreso, IngresoDto>, IIngresoReadRepository
    {
        public IngresoReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "Ingresos")
        {
        }

    }
}