using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Infrastructure.Persistence.Data.FormasPago
{

    // Nota: Asegúrate de que IFormaPagoWriteRepository herede de IWriteRepository<FormaPago>
    public class FormaPagoWriteRepository : AbsWriteRepository<FormaPago>, IWriteRepository<FormaPago>
    {
        public FormaPagoWriteRepository(AhorroLandDbContext context) : base(context)
        {
        }
    }
}
