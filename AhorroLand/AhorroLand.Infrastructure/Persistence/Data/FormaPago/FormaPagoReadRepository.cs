using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.FormasPago
{
    public class FormaPagoReadRepository : AbsReadRepository<FormaPago, FormaPagoDto>, IFormaPagoReadRepository
    {
        public FormaPagoReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "FormasPago")
        {
        }

    }
}