using AhorroLand.Domain.Conceptos;
using AhorroLand.Domain.Cuentas;
using AhorroLand.Domain.FormasPago;
using AhorroLand.Domain.Gastos.Events;
using AhorroLand.Domain.Ingresos.Events;
using AhorroLand.Domain.Personas;
using AhorroLand.Domain.Proveedores;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.Ingresos;

public sealed class Ingreso : AbsEntity
{
    private Ingreso(
        Guid id,
        Cantidad importe,
        FechaRegistro fecha,
        Concepto concepto,
        Proveedor proveedor,
        Persona persona,
        Cuenta cuenta,
        FormaPago formaPago,
        Descripcion? descripcion) : base(id)
    {
        Importe = importe;
        Fecha = fecha;
        Concepto = concepto;
        Proveedor = proveedor;
        Persona = persona;
        Cuenta = cuenta;
        FormaPago = formaPago;
        Descripcion = descripcion;
    }

    public Cantidad Importe { get; private set; }
    public FechaRegistro Fecha { get; private set; }
    public Descripcion? Descripcion { get; private set; }
    public Concepto Concepto { get; private set; }
    public Proveedor Proveedor { get; private set; }
    public Persona Persona { get; private set; }
    public Cuenta Cuenta { get; private set; }
    public FormaPago FormaPago { get; private set; }

    public static Ingreso Create(
        Guid id,
        decimal importe,
        DateTime fecha,
        Concepto concepto,
        Proveedor proveedor,
        Persona persona,
        Cuenta cuenta,
        FormaPago formaPago,
        string? descripcion)
    {

        if (concepto == null || cuenta == null)
        {
            throw new ArgumentNullException("Concepto y Cuenta son obligatorios.");
        }

        var importeVO = new Cantidad(importe);
        var fechaVO = new FechaRegistro(fecha);
        var descripcionVO = descripcion != null
            ? (Descripcion?)new Descripcion(descripcion)
            : null;

        var ingreso = new Ingreso(
            id,
            importeVO,
            fechaVO,
            concepto,
            proveedor,
            persona,
            cuenta,
            formaPago,
            descripcionVO);

        ingreso.RaiseDomainEvent(new IngresoCreatedDomainEvent(ingreso.Id));

        return ingreso;
    }

}