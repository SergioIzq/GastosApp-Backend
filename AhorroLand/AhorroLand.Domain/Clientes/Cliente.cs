using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain;

public sealed class Cliente : AbsEntity
{
    public Cliente() : base(Guid.Empty)
    {

    }

    private Cliente(Guid id, Nombre nombre, UsuarioId usuarioId) : base(id)
    {
        Nombre = nombre;
        UsuarioId = usuarioId;
    }

    public Nombre Nombre { get; private set; }
    public UsuarioId UsuarioId { get; private set; }

    public static Cliente Create(Nombre nombre, UsuarioId usuarioId)
    {
        var cliente = new Cliente(Guid.NewGuid(), nombre, usuarioId);

        return cliente;
    }

    public void Update(Nombre nombre) => Nombre = nombre;
}
