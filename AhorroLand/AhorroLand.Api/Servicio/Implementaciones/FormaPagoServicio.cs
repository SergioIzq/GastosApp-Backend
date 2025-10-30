using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;

namespace AppG.Servicio
{
    public class FormaPagoServicio : BaseServicio<FormaPago>, IFormaPagoServicio
    {
        public FormaPagoServicio(ISessionFactory sessionFactory) : base(sessionFactory) { }


        public override async Task<FormaPago> CreateAsync(FormaPago entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                // Verificar si la categoría existe en la base de datos
                var existingFormaPago = await session.Query<FormaPago>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingFormaPago != null && existingFormaPago.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"La forma de pago '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Guardar la entidad y guardar los cambios
                session.Save(entity);
                await transaction.CommitAsync();

                var id = session.GetIdentifier(entity);
                var createdEntity = await session.GetAsync<FormaPago>(id);

                return createdEntity;


            }
        }

        public override async Task UpdateAsync(int id, FormaPago entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingFormaPago = await session.Query<FormaPago>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingFormaPago != null && existingFormaPago.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El cliente '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                session.Update(entity);
                await transaction.CommitAsync();

            }
        }

        public override Task DeleteAsync(int id)
        {
            return base.DeleteAsync(id);
        }
    }
}

