using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using AppG.BBDD.Respuestas.Gastos;
using Hangfire;

namespace AppG.Servicio
{
    public class GastoProgramadoServicio : BaseServicio<GastoProgramado>, IGastoProgramadoServicio
    {
        public GastoProgramadoServicio(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }


        public async override Task<GastoProgramado> CreateAsync(GastoProgramado entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Obtener la categoría del entity
                    var entityCategoria = entity?.Concepto.Categoria;
                    if (entityCategoria != null)
                    {
                        // Verificar si la categoría existe en la base de datos
                        var existingCategoria = await session.Query<Categoria>()
                            .Where(c => c.Nombre == entityCategoria.Nombre && c.IdUsuario == entityCategoria.IdUsuario)
                            .SingleOrDefaultAsync();

                        if (existingCategoria != null)
                        {
                            // Asignar el ID de la categoría existente a la entidad
                            entity!.Concepto.Categoria = existingCategoria;
                        }
                        else
                        {
                            errorMessages.Add($"La categoría '{entityCategoria.Nombre}' no existe.");
                        }
                    }

                    // Guardar el gasto programado
                    session.Save(entity);
                    await transaction.CommitAsync();

                    var id = session.GetIdentifier(entity);
                    var createdEntity = await session.GetAsync<GastoProgramado>(id);

                    //var fechaEjecucion = CalcularFechaEjecucionDesdeDiaDelMes(entity.DiaEjecucion, entity.AjustarAUltimoDia);
                    //if (fechaEjecucion != null)
                    //{
                    //    var delay = fechaEjecucion.Value - DateTime.Now;
                    //    delay = TimeSpan.Zero;

                    //    BackgroundJob.Schedule<IGastoProgramadoServicio>(
                    //        x => x.AplicarGasto(createdEntity.Id),
                    //        delay
                    //    );
                    //}

                    return createdEntity;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }


        public override async Task UpdateAsync(int id, GastoProgramado entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Cargar la entidad existente
                    var existingEntity = await session.GetAsync<Gasto>(id);
                    if (existingEntity == null)
                    {
                        errorMessages.Add($"Entidad con ID {id} no encontrada");
                        throw new ValidationException(errorMessages);
                    }

                    // Obtener la categoría del entity
                    var entityCategoria = entity?.Concepto.Categoria;
                    if (entityCategoria != null)
                    {
                        // Verificar si la categoría existe en la base de datos
                        var existingCategoria = await session.Query<Categoria>()
                            .Where(c => c.Nombre == entityCategoria.Nombre && c.IdUsuario == entityCategoria.IdUsuario)
                            .SingleOrDefaultAsync();

                        if (existingCategoria != null)
                        {
                            // Asignar el ID de la categoría existente a la entidad
                            entity.Concepto.Categoria = existingCategoria;
                        }
                        else
                        {
                            errorMessages.Add($"La categoría '{entityCategoria.Nombre}' no existe.");
                        }
                    }

                    // Buscar la cuenta original del gasto (cuenta del gasto existente)
                    var originalCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == existingEntity.Cuenta.Nombre && c.IdUsuario == existingEntity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (originalCuenta == null)
                    {
                        errorMessages.Add($"La cuenta original '{existingEntity.Cuenta.Nombre}' no existe.");
                    }

                    // Buscar la nueva cuenta (cuenta del nuevo entity)
                    var nuevaCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.Cuenta.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (nuevaCuenta == null)
                    {
                        errorMessages.Add($"La cuenta '{entity.Cuenta.Nombre}' no existe.");
                    }

                    if (errorMessages.Count > 0)
                    {
                        throw new ValidationException(errorMessages);
                    }

                    // Revertir el saldo de la cuenta original basado en el gasto anterior
                    if (originalCuenta != null)
                    {
                        originalCuenta.Saldo += existingEntity.Monto;
                        session.Update(originalCuenta);
                    }

                    // Actualizar el saldo de la nueva cuenta basado en el nuevo monto
                    if (nuevaCuenta != null)
                    {
                        nuevaCuenta.Saldo -= entity.Monto;
                        session.Update(nuevaCuenta);
                    }

                    // Fusionar y guardar la entidad actualizada
                    session.Merge(entity);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Cargar la entidad existente
                    var existingEntity = await session.GetAsync<GastoProgramado>(id);
                    if (existingEntity == null)
                    {
                        errorMessages.Add($"Entidad con ID {id} no encontrada");
                        throw new ValidationException(errorMessages);
                    }

                    // Buscar la cuenta correspondiente por nombre
                    var cuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == existingEntity.Cuenta.Nombre && c.IdUsuario == existingEntity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuenta == null)
                    {
                        errorMessages.Add($"La cuenta '{existingEntity.Cuenta.Nombre}' no existe.");
                        throw new ValidationException(errorMessages);
                    }

                    cuenta.Saldo += existingEntity.Monto;

                    // Guardar la cuenta actualizada
                    session.Update(cuenta);

                    // Eliminar el gasto
                    await session.DeleteAsync(existingEntity);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        // Monta objeto para crear un nuevo gasto
        public async Task<GastoRespuesta> GetNewGastoAsync(int idUsuario)
        {
            var errorMessages = new List<string>();
            GastoRespuesta newGasto = new GastoRespuesta();

            using (var session = _sessionFactory.OpenSession())
            {
                try
                {
                    var listaCategorias = await session.Query<Categoria>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaConceptos = await session.Query<Concepto>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaCuentas = await session.Query<Cuenta>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaProveedores = await session.Query<Proveedor>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaPersonas = await session.Query<Persona>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaFormasPago = await session.Query<FormaPago>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    // Crear objeto respuesta al frontend para nuevos gastos
                    newGasto.ListaProveedores = listaProveedores;
                    newGasto.ListaCuentas = listaCuentas;
                    newGasto.ListaConceptos = listaConceptos;
                    newGasto.ListaCategorias = listaCategorias;
                    newGasto.ListaPersonas = listaPersonas;
                    newGasto.ListaFormasPago = listaFormasPago;

                    return newGasto;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<GastoProgramadoByIdRespuesta> GetGastoByIdAsync(int id)
        {
            GastoProgramadoByIdRespuesta response = new GastoProgramadoByIdRespuesta();

            response.GastoById = await base.GetByIdAsync(id);

            if (response.GastoById?.Cuenta?.IdUsuario != null)
            {
                response.GastoRespuesta = await GetNewGastoAsync(response.GastoById.Cuenta.IdUsuario);
            }

            return response;
        }

        public async Task AplicarGasto(int gastoProgramadoId)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var gasto = await session.GetAsync<GastoProgramado>(gastoProgramadoId);
                    if (gasto == null)
                    {
                        throw new Exception($"No se encontró el gasto programado con ID {gastoProgramadoId}");
                    }

                    if (!gasto.Activo)
                        return;

                    var cuenta = await session.Query<Cuenta>()
                        .Where(c => c.Id == gasto.Cuenta.Id && c.IdUsuario == gasto.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuenta == null)
                    {
                        throw new Exception($"No se encontró la cuenta con ID {gasto.Cuenta.Id}");
                    }

                    cuenta.Saldo -= Math.Abs(gasto.Monto);

                    session.Update(cuenta);                    

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}