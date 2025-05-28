using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;

namespace AppG.Servicio
{
    public class CategoriaServicio : BaseServicio<Categoria>, ICategoriaServicio
    {
        private readonly IIngresoServicio _ingresoServicio;
        private readonly IGastoProgramadoServicio _gastoProgramadoServicio;
        private readonly IGastoServicio _gastoServicio;
        public CategoriaServicio(ISessionFactory sessionFactory, IIngresoServicio ingresoServicio, IGastoProgramadoServicio gastoProgramadoServicio, IGastoServicio gastoServicio) : base(sessionFactory)
        {
            _ingresoServicio = ingresoServicio;
            _gastoProgramadoServicio = gastoProgramadoServicio;
            _gastoServicio = gastoServicio;
        }

        public override async Task<Categoria> CreateAsync(Categoria entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingCategoria = await session.Query<Categoria>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCategoria != null && existingCategoria.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"La categoría '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                session.Save(entity);
                await transaction.CommitAsync();

                var id = session.GetIdentifier(entity);
                var createdEntity = await session.GetAsync<Categoria>(id);

                return createdEntity;

            }
        }

        public override async Task UpdateAsync(int id, Categoria entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingCategoria = await session.Query<Categoria>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && entity.IdUsuario == c.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCategoria != null && existingCategoria.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"La categoría '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Guardar la entidad y guardar los cambios
                session.Update(entity);
                await transaction.CommitAsync();

            }
        }

        public override async Task DeleteAsync(int id)
        {
            var categoria = await GetByIdAsync(id);

            if (categoria == null)
            {
                throw new KeyNotFoundException("Categoría no encontrado");
            }

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // 2. Obtener las entidades relacionadas
                var ingresos = await session.Query<Ingreso>()
                                .Where(c => c.Concepto.Categoria.Id == id)
                                .ToListAsync();
                var gastos = await session.Query<Gasto>()
                                .Where(c => c.Concepto.Categoria.Id == id)
                                .ToListAsync();
                var gastosProgramados = await session.Query<GastoProgramado>()
                                .Where(c => c.Concepto.Categoria.Id == id)
                                .ToListAsync();

                // 3. Eliminar las entidades relacionadas (ingresos, gastos, traspasos)
                foreach (var ingreso in ingresos)
                {
                    await _ingresoServicio.DeleteAsync(ingreso.Id);
                }

                foreach (var gasto in gastos)
                {
                    await _gastoServicio.DeleteAsync(gasto.Id);
                }

                foreach (var gastoP in gastosProgramados)
                {
                    await _gastoProgramadoServicio.DeleteAsync(gastoP.Id);
                }
            }
            await base.DeleteAsync(id);
        }
    }
}

