using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.Conceptos
{

    // Nota: Asegúrate de que IConceptoWriteRepository herede de IWriteRepository<Concepto>
    public class ConceptoWriteRepository : AbsWriteRepository<Concepto>, IConceptoWriteRepository
    {
        public ConceptoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
