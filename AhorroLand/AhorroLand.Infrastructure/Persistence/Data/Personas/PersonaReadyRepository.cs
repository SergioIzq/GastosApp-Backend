using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Personas
{
    public class PersonaReadRepository : AbsReadRepository<Persona, PersonaDto>, IPersonaReadRepository
    {
        public PersonaReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "Personas")
        {
        }

    }
}