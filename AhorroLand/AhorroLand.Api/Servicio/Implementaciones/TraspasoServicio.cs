using AppG.BBDD.Respuestas.Traspasos;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;

namespace AppG.Servicio
{
    public class TraspasoServicio : BaseServicio<Traspaso>, ITraspasoServicio
    {
        public TraspasoServicio(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public async Task<TraspasoByIdRespuesta> GetTraspasoByIdAsync(int id)
        {
            TraspasoByIdRespuesta response = new TraspasoByIdRespuesta();
            var listaCuentas = new List<Cuenta>();

            response.TraspasoById = await base.GetByIdAsync(id);

            if (response.TraspasoById?.IdUsuario != null)
            {
                var idUsuario = response.TraspasoById.IdUsuario;
                using (var session = _sessionFactory.OpenSession())
                {
                    listaCuentas = await session.Query<Cuenta>()
                                        .Where(c => c.IdUsuario == idUsuario)
                                        .OrderBy(c => c.Nombre)
                                        .ToListAsync();
                }
                response.ListaCuentas = listaCuentas;
            }

            return response;
        }

        public async Task<List<Cuenta>> GetNewTraspasoAsync(int id)
        {
            var listaCuentas = new List<Cuenta>();

            if (id > 0)
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    listaCuentas = await session.Query<Cuenta>()
                                        .Where(c => c.IdUsuario == id)
                                        .OrderBy(c => c.Nombre)
                                        .ToListAsync();
                }
            }

            return listaCuentas;
        }

        public async Task<Traspaso> RealizarTraspaso(Traspaso entity, bool esProgramado = false)
        {
            IList<string> errorMessages = new List<string>();
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Buscar la cuenta de origen por nombre
                    var cuentaOrigen = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaOrigen.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaOrigen == null)
                    {
                        errorMessages.Add($"La cuenta de origen '{entity.CuentaOrigen.Nombre}' no existe.");
                    }

                    // Buscar la cuenta de destino por nombre
                    var cuentaDestino = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaDestino.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaDestino == null)
                    {
                        errorMessages.Add($"La cuenta de destino '{entity.CuentaDestino.Nombre}' no existe.");
                    }

                    if (!esProgramado)
                    {
                        // Realizar el traspaso
                        cuentaOrigen!.Saldo -= entity.Importe;

                        cuentaDestino!.Saldo += entity.Importe;
                    }

                    Traspaso traspaso = new Traspaso
                    {
                        CuentaOrigen = cuentaOrigen!,
                        SaldoCuentaOrigen = cuentaOrigen!.Saldo,
                        CuentaDestino = cuentaDestino!,
                        SaldoCuentaDestino = cuentaDestino!.Saldo,
                        Fecha = entity.Fecha,
                        Descripcion = entity.Descripcion,
                        Importe = entity.Importe,
                        IdUsuario = entity.CuentaOrigen.IdUsuario
                    };

                    if (!esProgramado)
                    {
                        // Guardar los cambios en las cuentas
                        await session.SaveOrUpdateAsync(cuentaOrigen);
                        await session.SaveOrUpdateAsync(cuentaDestino);
                    }
                    await session.SaveAsync(traspaso);

                    await transaction.CommitAsync();

                    var id = session.GetIdentifier(traspaso);
                    var createdEntity = await session.GetAsync<Traspaso>(id);

                    return createdEntity;
                }
                catch (Exception)
                {
                    // Rollback de la transacción en caso de error
                    await transaction.RollbackAsync();
                    throw new ValidationException(errorMessages);
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var entity = await session.GetAsync<Traspaso>(id);
                if (entity == null)
                {
                    throw new KeyNotFoundException($"Entidad con ID {id} no encontrada");
                }

                entity.CuentaOrigen.Saldo += entity.Importe;
                entity.CuentaDestino.Saldo -= entity.Importe;

                session.Delete(entity);
                await transaction.CommitAsync();
                session.Clear();
            }
        }

        public override async Task UpdateAsync(int id, Traspaso entity)
        {
            IList<string> errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Obtener el traspaso existente por ID
                    var existingTraspaso = await session.Query<Traspaso>()
                        .Where(t => t.Id == id)
                        .SingleOrDefaultAsync();

                    if (existingTraspaso == null)
                    {
                        throw new Exception($"El traspaso con ID '{id}' no existe.");
                    }

                    // Obtener las cuentas de origen y destino por nombre
                    var cuentaOrigen = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaOrigen.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaOrigen == null)
                    {
                        errorMessages.Add($"La cuenta de origen '{entity.CuentaOrigen.Nombre}' no existe.");
                    }

                    var cuentaDestino = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaDestino.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaDestino == null)
                    {
                        errorMessages.Add($"La cuenta de destino '{entity.CuentaDestino.Nombre}' no existe.");
                    }

                    if (errorMessages.Any())
                    {
                        throw new ValidationException(errorMessages);
                    }

                    // Actualizar los saldos de las cuentas
                    // Revertir el saldo anterior de las cuentas involucradas
                    cuentaOrigen!.Saldo += existingTraspaso.Importe;
                    cuentaDestino!.Saldo -= existingTraspaso.Importe;

                    // Aplicar el saldo actual del nuevo traspaso
                    cuentaOrigen.Saldo -= entity.Importe;
                    cuentaDestino.Saldo += entity.Importe;

                    // Actualizar los saldos en la entidad de traspaso
                    entity.SaldoCuentaOrigen = cuentaOrigen.Saldo;
                    entity.SaldoCuentaDestino = cuentaDestino.Saldo;

                    // Actualizar la entidad traspaso existente con la nueva información
                    existingTraspaso.Fecha = entity.Fecha;
                    existingTraspaso.Importe = entity.Importe;
                    existingTraspaso.Descripcion = entity.Descripcion;
                    existingTraspaso.CuentaOrigen = entity.CuentaOrigen;
                    existingTraspaso.CuentaDestino = entity.CuentaDestino;
                    existingTraspaso.SaldoCuentaOrigen = entity.SaldoCuentaOrigen;
                    existingTraspaso.SaldoCuentaDestino = entity.SaldoCuentaDestino;

                    // Guardar las actualizaciones en las cuentas y en el traspaso
                    await session.SaveOrUpdateAsync(cuentaOrigen);
                    await session.SaveOrUpdateAsync(cuentaDestino);
                    await session.SaveOrUpdateAsync(existingTraspaso);

                    // Commit de la transacción
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // Rollback en caso de error
                    await transaction.RollbackAsync();
                    throw new ValidationException(errorMessages);
                }
            }
        }

    }
}
