using AhorroLandBackend.Domain.Abstractions;
using AhorroLandBackend.Domain.Personas.Events;

namespace AhorroLandBackend.Domain.Personas;

public sealed class Persona : AbsEntity
{
    private Persona(Guid id): base(id)
    {
        
    }

    public Nombre? Nombre { get; private set; }

    public static Persona Create(Guid id, Nombre nombre)
    {
        var persona = new Persona(id)
        {
            Nombre = nombre
        };

        persona.RaiseDomainEvent(new PersonaCreatedDomainEvent(persona.Id));

        return persona;
    }
}
