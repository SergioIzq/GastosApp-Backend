using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.Conceptos
{

    // Nota: Asegúrate de que IConceptoWriteRepository herede de IWriteRepository<Concepto>
    public class ConceptoWriteRepository : AbsWriteRepository<Concepto>, IWriteRepository<Concepto>
    {
        public ConceptoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
