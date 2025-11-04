using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;

namespace AppG.Servicio
{
    public class ProveedorServicio : BaseServicio<Proveedor>, IProveedorServicio
    {
        private readonly IGastoServicio _gastoServicio;
        private readonly IGastoProgramadoServicio _gastoProgramadosServicio;
        public ProveedorServicio(ISessionFactory sessionFactory, IGastoServicio gastoServicio, IGastoProgramadoServicio gastoProgramadoServicio) : base(sessionFactory)
        {
            _gastoProgramadosServicio = gastoProgramadoServicio;
            _gastoServicio = gastoServicio;
        }


        public override async Task<Proveedor> CreateAsync(Proveedor entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                // Verificar si la categoría existe en la base de datos
                var existingProveedor = await session.Query<Proveedor>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingProveedor != null && existingProveedor.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El proveedor '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }


                try
                {
                    // Guardar la entidad y guardar los cambios
                    session.Save(entity);
                    await transaction.CommitAsync();

                    var id = session.GetIdentifier(entity);
                    var createdEntity = await session.GetAsync<Proveedor>(id);

                    return createdEntity;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }

            }
        }

        public override async Task UpdateAsync(int id, Proveedor entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingCliente = await session.Query<Proveedor>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCliente != null && existingCliente.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El proveedor '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }


                try
                {
                    // Guardar la entidad y guardar los cambios
                    session.Update(entity);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }

            }
        }

        public override async Task DeleteAsync(int id)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var gastos = await session.Query<Gasto>()
                            .Where(c => c.Concepto.Categoria.Id == id)
                            .ToListAsync();

                var gastosProgramados = await session.Query<GastoProgramado>()
                            .Where(c => c.Concepto.Categoria.Id == id)
                            .ToListAsync();

                foreach (var gasto in gastos)
                {
                    await _gastoServicio.DeleteAsync(gasto.Id);
                }

                foreach (var gastoProgramado in gastosProgramados)
                {
                    await _gastoProgramadosServicio.DeleteAsync(gastoProgramado.Id);
                }
            }

            await base.DeleteAsync(id);
        }
    }
}

