using AhorroLand.Domain.Personas.Events;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.Personas;

public sealed class Persona : AbsEntity
{
    private Persona(Guid id, Nombre nombre): base(id)
    {
        Nombre = nombre;
    }

    public Nombre Nombre { get; private set; }

    public static Persona Create(Guid id, Nombre nombre)
    {
        var persona = new Persona(id, nombre);

        persona.RaiseDomainEvent(new PersonaCreatedDomainEvent(persona.Id));

        return persona;
    }
}
