using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Conceptos
{
    public class ConceptoReadRepository : AbsReadRepository<Concepto, ConceptoDto>, IConceptoReadRepository
    {
        public ConceptoReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "Conceptos")
        {
        }

    }
}