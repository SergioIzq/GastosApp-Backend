using AhorroLand.Domain.Categorias.Events;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.Categorias;

public sealed class Categoria : AbsEntity
{
    private Categoria(Guid id, Nombre nombre, Descripcion? descripcion = null) : base(id)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public Nombre Nombre { get; private set; }
    public Descripcion? Descripcion { get; private set; }

    public static Categoria Create(Guid id, Nombre nombre, Descripcion? descripcion = null)
    {
        var categoria = new Categoria(id, nombre, descripcion);

        categoria.RaiseDomainEvent(new CategoriaCreatedDomainEvent(categoria.Id));

        return categoria;
    }

    /// <summary>
    /// Actualiza el nombre y la descripción de la categoría.
    /// </summary>
    /// <param name="nombre">El nuevo Value Object Nombre (ya validado).</param>
    /// <param name="descripcion">El nuevo Value Object Descripcion.</param>
    public void Update(Nombre nombre, Descripcion? descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;

        // 3. Opcional: Levantar un evento de dominio si la actualización es significativa.
        RaiseDomainEvent(new CategoriaUpdatedDomainEvent(Id));
    }
}
