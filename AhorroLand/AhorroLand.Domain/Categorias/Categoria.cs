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
}
