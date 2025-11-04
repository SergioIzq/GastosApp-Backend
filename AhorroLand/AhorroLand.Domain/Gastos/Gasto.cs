using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain;

public sealed class Gasto : AbsEntity
{
    // El constructor sigue siendo PURO.
    // Solo acepta los IDs, nunca las entidades completas.
    private Gasto(
        Guid id,
        Cantidad importe,
        FechaRegistro fecha,
        ConceptoId conceptoId,
        CategoriaId categoriaId,
        ProveedorId proveedorId,
        PersonaId personaId,
        CuentaId cuentaId,
        FormaPagoId formaPagoId,
        UsuarioId usuarioId,
        Descripcion? descripcion) : base(id)
    {
        Importe = importe;
        Fecha = fecha;

        ConceptoId = conceptoId;
        CategoriaId = categoriaId;
        ProveedorId = proveedorId;
        PersonaId = personaId;
        CuentaId = cuentaId;
        FormaPagoId = formaPagoId;
        UsuarioId = usuarioId;

        Descripcion = descripcion;
    }

    // --- Propiedades Puras del Dominio ---
    public Cantidad Importe { get; private set; }
    public FechaRegistro Fecha { get; private set; }
    public Descripcion? Descripcion { get; private set; }

    // --- IDs (Referencias a otros Agregados) ---
    public ConceptoId ConceptoId { get; private set; }
    public CategoriaId CategoriaId { get; private set; }
    public ProveedorId ProveedorId { get; private set; }
    public PersonaId PersonaId { get; private set; }
    public CuentaId CuentaId { get; private set; }
    public FormaPagoId FormaPagoId { get; private set; }
    public UsuarioId UsuarioId { get; private set; }

    // --- Detalles de Infraestructura (Solo para Proyecciones/Queries) ---
    // Propiedades de navegación para que EF Core y Mapster generen JOINs.
    // El dominio (métodos de negocio) NUNCA debe usarlas.
    public Concepto Concepto { get; private set; } = null!;
    public Categoria Categoria { get; private set; } = null!;
    public Proveedor Proveedor { get; private set; } = null!;
    public Persona Persona { get; private set; } = null!;
    public Cuenta Cuenta { get; private set; } = null!;
    public FormaPago FormaPago { get; private set; } = null!;
    public Usuario Usuario { get; private set; } = null!;
    // --- Fin Detalles de Infraestructura ---


    // El método Factory (Create) sigue siendo PURO.
    public static Gasto Create(
        Cantidad importe,
        FechaRegistro fecha,
        ConceptoId conceptoId,
        CategoriaId categoriaId,
        ProveedorId proveedorId,
        PersonaId personaId,
        CuentaId cuentaId,
        FormaPagoId formaPagoId,
        UsuarioId usuarioId,
        Descripcion? descripcion)
    {
        var gasto = new Gasto(
            Guid.NewGuid(),
            importe,
            fecha,
            conceptoId,
            categoriaId,
            proveedorId,
            personaId,
            cuentaId,
            formaPagoId,
            usuarioId,
            descripcion);

        return gasto;
    }
}