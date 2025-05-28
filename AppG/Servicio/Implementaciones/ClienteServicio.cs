using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;

namespace AppG.Servicio
{
    public class ClienteServicio : BaseServicio<Cliente>, IClienteServicio
    {
        private readonly IIngresoServicio _ingresoServicio;
        public ClienteServicio(ISessionFactory sessionFactory, IIngresoServicio ingresoServicio) : base(sessionFactory)
        {
            _ingresoServicio = ingresoServicio;
        }


        public override async Task<Cliente> CreateAsync(Cliente entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                // Verificar si la categoría existe en la base de datos
                var existingCliente = await session.Query<Cliente>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCliente != null && existingCliente.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El cliente '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Guardar la entidad y guardar los cambios
                session.Save(entity);
                await transaction.CommitAsync();

                var id = session.GetIdentifier(entity);
                var createdEntity = await session.GetAsync<Cliente>(id);

                return createdEntity;
            }
        }

        public override async Task UpdateAsync(int id, Cliente entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingCliente = await session.Query<Cliente>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCliente != null && existingCliente.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El cliente '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Guardar la entidad y guardar los cambios
                session.Update(entity);
                await transaction.CommitAsync();
            }
        }

        public async override Task DeleteAsync(int id)
        {            
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var ingresos = await session.Query<Ingreso>()
                            .Where(c => c.Concepto.Categoria.Id == id)
                            .ToListAsync();

                foreach (var ingreso in ingresos)
                {
                    await _ingresoServicio.DeleteAsync(ingreso.Id);
                }
            }

            await base.DeleteAsync(id);
        }
    }
}

