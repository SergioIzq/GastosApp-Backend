using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Personas
{

    // Nota: Asegúrate de que IPersonaWriteRepository herede de IWriteRepository<Persona>
    public class PersonaWriteRepository : AbsWriteRepository<Persona>, IPersonaWriteRepository
    {
        public PersonaWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
