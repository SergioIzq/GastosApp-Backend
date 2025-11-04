using AhorroLand.Shared.Domain.Abstractions; // Asegúrate de que esta asamblea sea accesible
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AhorroLand.Infrastructure.Persistence.Command;

public class AhorroLandDbContext : DbContext
{
    public AhorroLandDbContext(DbContextOptions<AhorroLandDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Obtener la assembly (proyecto) que contiene las entidades de dominio.
        // Usamos typeof(AbsEntity) o typeof(Cliente) para obtener una referencia a esa assembly.
        var domainAssembly = Assembly.GetAssembly(typeof(AbsEntity));

        if (domainAssembly == null)
        {
            throw new InvalidOperationException("El assembly de Dominio no se pudo cargar.");
        }

        // 2. Escanear la assembly para encontrar todas las clases que son Entidades.
        // Filtramos por clases que no son abstractas y que heredan de AbsEntity.
        var entityTypes = domainAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(AbsEntity)));

        // 3. Registrar cada entidad encontrada en el modelo de EF Core.
        foreach (var type in entityTypes)
        {
            modelBuilder.Entity(type);
        }

        modelBuilder.ApplyConfigurationsFromAssembly(domainAssembly);

        base.OnModelCreating(modelBuilder);
    }
}