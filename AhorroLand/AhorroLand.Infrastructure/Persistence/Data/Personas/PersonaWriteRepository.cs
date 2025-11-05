using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.Personas
{

    // Nota: Asegúrate de que IPersonaWriteRepository herede de IWriteRepository<Persona>
    public class PersonaWriteRepository : AbsWriteRepository<Persona>, IWriteRepository<Persona>
    {
        public PersonaWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
