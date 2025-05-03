using AppG.Controllers;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using Microsoft.AspNetCore.Identity;
using NHibernate;
using NHibernate.Linq;

namespace AppG.Servicio
{
    public class UsuarioServicio : BaseServicio<Usuario>, IUsuarioServicio
    {
        public UsuarioServicio(ISessionFactory sessionFactory) : base(sessionFactory) { }

        public override async Task UpdateAsync(int id, Usuario entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Verificar si el usuario existe en la base de datos con un correo diferente
                var existingUsuario = await session.Query<Usuario>()
                    .Where(c => c.Correo == entity.Correo && c.Id != entity.Id)
                    .SingleOrDefaultAsync();

                if (existingUsuario != null && existingUsuario.Correo.ToLower() == entity.Correo.ToLower())
                {
                    // Agregar un mensaje de error si el correo ya existe
                    errorMessages.Add($"El correo '{entity.Correo}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Recuperar la entidad existente desde la base de datos
                var entidadExistente = await session.Query<Usuario>()
                    .Where(c => c.Id == entity.Id)
                    .SingleOrDefaultAsync();

                if (entidadExistente == null)
                {
                    errorMessages.Add($"El usuario con ID {id} no existe.");
                    throw new KeyNotFoundException();
                }


                var hasher = new PasswordHasher<Usuario>();


                // Verificar y actualizar la contraseña si es necesario
                if (entity.Contrasena != entidadExistente.Contrasena)
                {                    
                    entidadExistente.Contrasena = hasher.HashPassword(entity, entity.Contrasena);
                }
                entidadExistente.Correo = entity.Correo;                

                // Guardar los cambios en la base de datos
                session.Update(entidadExistente);
                await transaction.CommitAsync();
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Recuperar el usuario a eliminar
                    var usuario = await session.Query<Usuario>()
                        .SingleOrDefaultAsync(u => u.Id == id);

                    if (usuario == null)
                    {
                        throw new KeyNotFoundException($"El usuario con ID {id} no se encontró.");
                    }

                    // Recuperar y eliminar todas las entidades relacionadas
                    var conceptosRelacionadas = await session.Query<Concepto>()
                        .Where(e => e.IdUsuario == id && e.IdUsuario == id)
                        .ToListAsync();
                    foreach (var entidad in conceptosRelacionadas)
                    {
                        session.Delete(entidad);
                    }

                    var categoriasRelacionadas = await session.Query<Categoria>()
                        .Where(e => e.IdUsuario == id && e.IdUsuario == id)
                        .ToListAsync();
                    foreach (var entidad in categoriasRelacionadas)
                    {
                        session.Delete(entidad);
                    }

                    var clientesRelacionados = await session.Query<Cliente>()
                        .Where(e => e.IdUsuario == id && e.IdUsuario == id)
                        .ToListAsync();
                    foreach (var entidad in clientesRelacionados)
                    {
                        session.Delete(entidad);
                    }

                    var cuentasRelacionadas = await session.Query<Cuenta>()
                        .Where(e => e.IdUsuario == id && e.IdUsuario == id)
                        .ToListAsync();
                    foreach (var entidad in cuentasRelacionadas)
                    {
                        session.Delete(entidad);
                    }

                    var formasPagoRelacionados = await session.Query<FormaPago>()
                        .Where(e => e.IdUsuario == id && e.IdUsuario == id)
                        .ToListAsync();
                    foreach (var entidad in formasPagoRelacionados)
                    {
                        session.Delete(entidad);
                    }

                    var gastosRelacionados = await session.Query<Gasto>()
                        .Where(e => e.IdUsuario == id && e.IdUsuario == id)
                        .ToListAsync();
                    foreach (var entidad in gastosRelacionados)
                    {
                        session.Delete(entidad);
                    }

                    var ingresosRelacionados = await session.Query<Ingreso>()
                        .Where(e => e.IdUsuario == id && e.IdUsuario == id)
                        .ToListAsync();
                    foreach (var entidad in ingresosRelacionados)
                    {
                        session.Delete(entidad);
                    }

                    var personasRelacionadas = await session.Query<Persona>()
                        .Where(e => e.IdUsuario == id && e.IdUsuario == id)
                        .ToListAsync();
                    foreach (var entidad in personasRelacionadas)
                    {
                        session.Delete(entidad);
                    }

                    var proveedorRelacionados = await session.Query<Proveedor>()
                    .Where(e => e.IdUsuario == id && e.IdUsuario == id)
                    .ToListAsync();
                    foreach (var entidad in proveedorRelacionados)
                    {
                        session.Delete(entidad);
                    }

                    // Eliminar el usuario
                    session.Delete(usuario);

                    // Guardar los cambios
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones
                    transaction.Rollback();
                    throw new InvalidOperationException("Error al eliminar el usuario y sus entidades relacionadas.", ex);
                }
            }
        }

    }
}

