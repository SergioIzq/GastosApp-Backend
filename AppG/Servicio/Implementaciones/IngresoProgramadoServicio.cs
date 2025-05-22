using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using AppG.BBDD.Respuestas.Ingresos;
using Hangfire;
using System.Globalization;

namespace AppG.Servicio
{
    public class IngresoProgramadoServicio : BaseServicio<IngresoProgramado>, IIngresoProgramadoServicio
    {
        private readonly IIngresoServicio _ingresoServicio;
        private readonly EmailService _emailService;
        public IngresoProgramadoServicio(ISessionFactory sessionFactory, IIngresoServicio ingresoServicio, EmailService emailService) : base(sessionFactory)
        {
            _ingresoServicio = ingresoServicio;
            _emailService = emailService;
        }


        public async override Task<IngresoProgramado> CreateAsync(IngresoProgramado entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Obtener la categoría del entity
                    var entityCategoria = entity?.Concepto!.Categoria;
                    if (entityCategoria != null)
                    {
                        // Verificar si la categoría existe en la base de datos
                        var existingCategoria = await session.Query<Categoria>()
                            .Where(c => c.Nombre == entityCategoria.Nombre && c.IdUsuario == entityCategoria.IdUsuario)
                            .SingleOrDefaultAsync();

                        if (existingCategoria != null)
                        {
                            // Asignar el ID de la categoría existente a la entidad
                            entity!.Concepto!.Categoria = existingCategoria;
                        }
                        else
                        {
                            errorMessages.Add($"La categoría '{entityCategoria.Nombre}' no existe.");
                        }
                    }

                    // Guardar el ingreso programado
                    session.Save(entity);
                    await transaction.CommitAsync();

                    // Obtener el ID generado
                    var id = session.GetIdentifier(entity);

                    // Recuperar la entidad completa (por si se llenan otros campos en base de datos)
                    var createdEntity = await session.GetAsync<IngresoProgramado>(id);

                    // Programar el trabajo recurrente en Hangfire
                    await ProgramarTrabajoRecurrente(createdEntity);

                    // Guardar el HangfireJobId en base de datos si fue modificado
                    using (var updateTransaction = session.BeginTransaction())
                    {
                        await session.UpdateAsync(createdEntity);
                        await updateTransaction.CommitAsync();
                    }

                    return createdEntity;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }


        public override async Task UpdateAsync(int id, IngresoProgramado entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Cargar la entidad existente
                    var existingEntity = await session.GetAsync<IngresoProgramado>(id);
                    if (existingEntity == null)
                    {
                        errorMessages.Add($"Entidad con ID {id} no encontrada");
                        throw new ValidationException(errorMessages);
                    }

                    // Obtener la categoría del entity
                    var entityCategoria = entity?.Concepto!.Categoria;
                    if (entityCategoria != null)
                    {
                        // Verificar si la categoría existe en la base de datos
                        var existingCategoria = await session.Query<Categoria>()
                            .Where(c => c.Nombre == entityCategoria.Nombre && c.IdUsuario == entityCategoria.IdUsuario)
                            .SingleOrDefaultAsync();

                        if (existingCategoria != null)
                        {
                            // Asignar el ID de la categoría existente a la entidad
                            entity!.Concepto!.Categoria = existingCategoria;
                        }
                        else
                        {
                            errorMessages.Add($"La categoría '{entityCategoria.Nombre}' no existe.");
                        }
                    }

                    // Buscar la cuenta original del ingreso (cuenta del ingreso existente)
                    var originalCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == existingEntity!.Cuenta!.Nombre && c.IdUsuario == existingEntity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (originalCuenta == null)
                    {
                        errorMessages.Add($"La cuenta original '{existingEntity!.Cuenta!.Nombre}' no existe.");
                    }

                    // Buscar la nueva cuenta (cuenta del nuevo entity)
                    var nuevaCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity!.Cuenta!.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (nuevaCuenta == null)
                    {
                        errorMessages.Add($"La cuenta '{entity!.Cuenta!.Nombre}' no existe.");
                    }

                    if (errorMessages.Count > 0)
                    {
                        throw new ValidationException(errorMessages);
                    }

                    // Fusionar y guardar la entidad actualizada
                    session.Merge(entity);
                    await transaction.CommitAsync();

                    // Reprogramar Hangfire
                    await ProgramarTrabajoRecurrente(entity!);
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
                    var existingEntity = await session.GetAsync<IngresoProgramado>(id);
                    if (existingEntity == null)
                    {
                        errorMessages.Add($"Entidad con ID {id} no encontrada");
                        throw new ValidationException(errorMessages);
                    }

                    // Eliminar el job de Hangfire si existe
                    if (!string.IsNullOrEmpty(existingEntity.HangfireJobId))
                    {
                        RecurringJob.RemoveIfExists(existingEntity.HangfireJobId);
                    }

                    // Eliminar el ingreso
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

        // Monta objeto para crear un nuevo ingreso
        public async Task<IngresoRespuesta> GetNewIngresoAsync(int idUsuario)
        {
            var errorMessages = new List<string>();
            IngresoRespuesta newIngreso = new IngresoRespuesta();

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

                    var listaClientes = await session.Query<Cliente>()
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

                    // Crear objeto respuesta al frontend para nuevos ingresos
                    newIngreso.ListaClientes = listaClientes;
                    newIngreso.ListaCuentas = listaCuentas;
                    newIngreso.ListaConceptos = listaConceptos;
                    newIngreso.ListaCategorias = listaCategorias;
                    newIngreso.ListaPersonas = listaPersonas;
                    newIngreso.ListaFormasPago = listaFormasPago;

                    return newIngreso;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IngresoProgramadoByIdRespuesta> GetIngresoByIdAsync(int id)
        {
            IngresoProgramadoByIdRespuesta response = new IngresoProgramadoByIdRespuesta();

            response.IngresoProgramadoById = await base.GetByIdAsync(id);

            if (response.IngresoProgramadoById?.Cuenta?.IdUsuario != null)
            {
                response.IngresoRespuesta = await GetNewIngresoAsync(response.IngresoProgramadoById.Cuenta.IdUsuario);
            }

            return response;
        }

        public async Task AplicarIngreso(int ingresoProgramadoId)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var ingresoP = await session.GetAsync<IngresoProgramado>(ingresoProgramadoId);
                    if (ingresoP == null)
                    {
                        throw new Exception($"No se encontró el ingreso programado con ID {ingresoProgramadoId}");
                    }

                    if (!ingresoP.Activo)
                        return;

                    var cuenta = await session.Query<Cuenta>()
                        .Where(c => c.Id == ingresoP!.Cuenta!.Id && c.IdUsuario == ingresoP.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuenta == null)
                    {
                        throw new Exception($"No se encontró la cuenta con ID {ingresoP!.Cuenta!.Id}");
                    }

                    cuenta.Saldo += Math.Abs(ingresoP.Monto);

                    session.Update(cuenta);

                    Ingreso ingreso = new Ingreso();
                    ingreso.Cliente = ingresoP.Cliente;
                    ingreso.Concepto = ingresoP.Concepto;
                    ingreso.Cuenta = ingresoP.Cuenta;
                    ingreso.Descripcion = ingresoP.Descripcion;
                    ingreso.Fecha = DateTime.Now;
                    ingreso.FormaPago = ingresoP.FormaPago;
                    ingreso.IdUsuario = ingresoP.IdUsuario;
                    ingreso.Monto = ingresoP.Monto;
                    ingreso.Persona = ingresoP.Persona;

                    await _ingresoServicio.CreateAsync(ingreso, true);                    
                    await transaction.CommitAsync();
                    await session.FlushAsync();

                    var usuario = await session.GetAsync<Usuario>(ingreso.IdUsuario);

                    var baseUrl = $"https://ahorroland.sergioizq.es/ingresos/ingreso-detail/{ingreso.Id}";
#if DEBUG
                    baseUrl = $"http://localhost:4200/ingresos/ingreso-detail/{ingreso.Id}";
#endif

                    await _emailService.SendEmailAsync(
                            usuario.Correo,
                            "Ingreso programado ejecutado - Ahorroland",
                            $@"
                            <html>
                              <body style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
                                <h1>Ingreso programado ejecutado correctamente</h1>
                                <p>Se ha aplicado automáticamente un ingreso programado con los siguientes detalles:</p>
                                <ul>
                                  <li><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
                                  <li><strong>Importe:</strong> +{ingreso.Monto.ToString("N2", new CultureInfo("es-ES"))} €</li>
                                  <li><strong>Cuenta:</strong> {ingreso.Cuenta.Nombre}</li>
                                  <li><strong>Categoria:</strong> {ingreso.Concepto.Categoria.Nombre}</li> 
                                  <li><strong>Concepto:</strong> {ingreso.Concepto.Nombre}</li>
                                </ul>
                                <p>Puedes ver el ingreso registrado en la sección <strong>Operaciones > Ingresos</strong> de tu cuenta:</p>
                                <p>
                                  <a href='{baseUrl}' target='_blank' 
                                     style='display: inline-block; padding: 10px 20px; background-color: #1a73e8; color: white; text-decoration: none; border-radius: 4px;'>
                                    Ver Ingreso
                                  </a>
                                </p>
                                <p style='margin-top: 20px;'>Si no reconoces este ingreso, por favor contacta con el administrador.</p>
                              </body>
                            </html>
                            "
                        );

                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private DateTime? CalcularFechaEjecucionDesdeDiaDelMes(DateTime fechaEjecucion, string frecuencia)
        {
            var hoy = DateTime.Today;
            var año = fechaEjecucion.Year;
            var mes = fechaEjecucion.Month;
            var dia = fechaEjecucion.Day;

            // Frecuencia Diaria
            if (frecuencia.Equals("DIARIA", StringComparison.OrdinalIgnoreCase))
            {
                DateTime fechaActual = DateTime.Now;

                // Obtener solo la fecha (sin hora) de la fecha actual
                DateTime fechaHoy = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day);

                // Si la hora de fechaEjecucion es posterior a la hora actual del día
                if (fechaEjecucion < fechaHoy.AddHours(fechaEjecucion.Hour).AddMinutes(fechaEjecucion.Minute))
                {
                    // Si la hora no ha pasado aún, programar para hoy a esa hora
                    return fechaHoy.AddHours(fechaEjecucion.Hour).AddMinutes(fechaEjecucion.Minute);
                }
                else
                {
                    // Si la hora ya ha pasado hoy, programar para mañana a esa hora
                    return fechaEjecucion.AddDays(1);
                }
            }

            // Frecuencia Mensual
            if (frecuencia.Equals("MENSUAL", StringComparison.OrdinalIgnoreCase))
            {
                // Si el día de ejecución es mayor que el día de hoy y aún no ha pasado, ejecutamos en este mes
                if (fechaEjecucion.Month == hoy.Month && fechaEjecucion.Day >= hoy.Day)
                {
                    // Si el día aún no ha pasado este mes, la ejecución es este mes
                    return new DateTime(hoy.Year, hoy.Month, fechaEjecucion.Day, fechaEjecucion.Hour, fechaEjecucion.Minute, 0);
                }

                // Si el día ya pasó o estamos en un mes posterior, programar para el siguiente mes
                mes++;
                if (mes > 12)
                {
                    mes = 1;
                    año++;
                }

                return new DateTime(año, mes, dia, fechaEjecucion.Hour, fechaEjecucion.Minute, 0);
            }

            if (frecuencia.Equals("SEMANAL", StringComparison.OrdinalIgnoreCase))
            {
                return fechaEjecucion;
            }

            // Si la frecuencia no es válida, devolver null
            return null;
        }

        private async Task ProgramarTrabajoRecurrente(IngresoProgramado createdEntity)
        {
            try
            {
                var recurringJobId = $"AplicarIngreso_{createdEntity.Id}";

                var fechaEjecucion = CalcularFechaEjecucionDesdeDiaDelMes(
                    createdEntity.FechaEjecucion,
                    createdEntity.Frecuencia
                );

                if (fechaEjecucion != null)
                {
                    var delay = fechaEjecucion.Value - DateTime.Now;

                    if (delay < TimeSpan.Zero)
                    {
                        if (createdEntity.Frecuencia.Equals("DIARIA", StringComparison.OrdinalIgnoreCase))
                        {
                            fechaEjecucion = fechaEjecucion.Value.AddDays(1);
                        }
                        else if (createdEntity.Frecuencia.Equals("SEMANAL", StringComparison.OrdinalIgnoreCase))
                        {
                            fechaEjecucion = fechaEjecucion.Value.AddDays(7);
                        }

                        delay = fechaEjecucion.Value - DateTime.Now;
                    }

                    createdEntity.HangfireJobId = recurringJobId;

                    var hora = fechaEjecucion.Value.Hour;
                    var minuto = fechaEjecucion.Value.Minute;

                    if (hora == 0 && minuto == 0)
                    {
                        hora = 8;
                        minuto = 0;
                    }

                    if (createdEntity.Frecuencia.Equals("DIARIA", StringComparison.OrdinalIgnoreCase))
                    {
                        RecurringJob.AddOrUpdate(
                            recurringJobId,
                            () => AplicarIngreso(createdEntity.Id),
                            Cron.Daily(hora, minuto),
                            new RecurringJobOptions
                            {
                                TimeZone = TimeZoneInfo.Local
                            }
                        );
                    }
                    else if (createdEntity.Frecuencia.Equals("SEMANAL", StringComparison.OrdinalIgnoreCase))
                    {
                        DayOfWeek dayOfWeek = fechaEjecucion.Value.DayOfWeek;

                        RecurringJob.AddOrUpdate(
                            recurringJobId,
                            () => AplicarIngreso(createdEntity.Id),
                            Cron.Weekly(dayOfWeek, hora, minuto),
                            new RecurringJobOptions
                            {
                                TimeZone = TimeZoneInfo.Local
                            }
                        );
                    }
                    else if (createdEntity.Frecuencia.Equals("MENSUAL", StringComparison.OrdinalIgnoreCase))
                    {
                        RecurringJob.AddOrUpdate(
                            recurringJobId,
                            () => AplicarIngreso(createdEntity.Id),
                            Cron.Monthly(fechaEjecucion.Value.Day, hora, minuto),
                            new RecurringJobOptions
                            {
                                TimeZone = TimeZoneInfo.Local
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                await DeleteAsync(createdEntity.Id);
                throw new Exception("No se ha podido programar el ingreso. " + ex.Message);
            }
        }
    }
}