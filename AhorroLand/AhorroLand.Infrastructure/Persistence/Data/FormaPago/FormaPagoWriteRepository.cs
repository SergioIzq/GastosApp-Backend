using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;

namespace AhorroLand.Infrastructure.Persistence.Data.FormasPago
{

    // Nota: Asegúrate de que IFormaPagoWriteRepository herede de IWriteRepository<FormaPago>
    public class FormaPagoWriteRepository : AbsWriteRepository<FormaPago>, IFormaPagoWriteRepository
    {
        public FormaPagoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
