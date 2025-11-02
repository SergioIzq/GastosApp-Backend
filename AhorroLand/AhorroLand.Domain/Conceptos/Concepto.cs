using AhorroLand.Domain.Categorias;
using AhorroLand.Domain.Conceptos.Events;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.Conceptos;

public sealed class Concepto : AbsEntity
{
    private Concepto(Guid id, Nombre nombre, Categoria categoria):base(id)
    {
        Nombre = nombre;
        Categoria = categoria;
    }

    public Nombre Nombre { get; private set; }
    public Categoria Categoria { get; private set; }

    public static Concepto Create(Guid id, Nombre nombre,  Categoria categoria)
    {
        var concepto = new Concepto(id, nombre, categoria);

        concepto.RaiseDomainEvent(new ConceptoCreatedDomainEvent(concepto.Id));

        return concepto;
    }
}
