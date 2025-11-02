using AhorroLand.Domain.Cuentas;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;
using AhorroLand.Domain.Traspasos.Events;

namespace AhorroLand.Domain.Traspasos;

public sealed class Traspaso : AbsEntity
{
    private Traspaso(
        Guid id,
        Cuenta cuentaOrigen,
        Cuenta cuentaDestino,
        Cantidad importe,
        FechaRegistro fecha,
        Descripcion? descripcion) : base(id)
    {
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
        Importe = importe;
        Fecha = fecha;
        Descripcion = descripcion;
    }

    public Cuenta CuentaOrigen { get; }
    public Cuenta CuentaDestino { get; }

    public Cantidad Importe { get; }
    public FechaRegistro Fecha { get; }
    public Descripcion? Descripcion { get; }

    public static Traspaso Create(
        Cuenta cuentaOrigen,
        Cuenta cuentaDestino,
        decimal importe,
        DateTime fecha,
        string? descripcion)
    {
        if (cuentaOrigen.Equals(cuentaDestino))
        {
            throw new InvalidOperationException("La cuenta de origen y destino deben ser diferentes.");
        }

        var importeVO = new Cantidad(importe);
        var fechaVO = new FechaRegistro(fecha);
        var descripcionVO = descripcion != null
                    ? (Descripcion?)new Descripcion(descripcion)
                    : null;

        // 🔑 LÓGICA DDD: La responsabilidad de los saldos es de la entidad Cuenta.
        // La Entidad Traspaso dispara los cambios en las cuentas.

        // 1. Actualizar saldos (asumiendo que los métodos Depositar/Retirar existen y validan)
        cuentaOrigen.Retirar(importeVO);
        cuentaDestino.Depositar(importeVO);

        // 2. Crear la entidad Traspaso (Registro de la acción)
        var traspaso = new Traspaso(
            Guid.NewGuid(),
            cuentaOrigen,
            cuentaDestino,
            importeVO,
            fechaVO,
            descripcionVO);

        traspaso.RaiseDomainEvent(new TraspasoCreatedDomainEvent(traspaso.Id));

        return traspaso;
    }
}