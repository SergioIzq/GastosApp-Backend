using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.TraspasoProgramados
{
    public class TraspasoProgramadoReadRepository : AbsReadRepository<TraspasoProgramado, TraspasoProgramadoDto>, ITraspasoProgramadoReadRepository
    {
        public TraspasoProgramadoReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "traspasoProgramados")
        {
        }

    }
}