using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Gastos
{
  public class GastoReadRepository : AbsReadRepository<Gasto, GastoDto>, IGastoReadRepository
 {
        public GastoReadRepository(IDbConnectionFactory dbConnectionFactory)
        : base(dbConnectionFactory, "gastos")
  {
    }

        /// <summary>
        /// 🔥 Alias de la tabla principal para usar en JOINs.
     /// </summary>
        protected override string GetTableAlias()
   {
            return "g";
 }

        /// <summary>
        /// 🔥 Query de conteo que usa el alias correcto.
        /// </summary>
        protected override string BuildCountQuery()
        {
       return "SELECT COUNT(*) FROM gastos g";
        }

  /// <summary>
        /// 🔥 Query optimizado para obtener un gasto con todos sus datos relacionados.
        /// </summary>
        protected override string BuildGetByIdQuery()
     {
    return @"
SELECT 
g.id as Id,
  g.importe as Importe,
    g.fecha as Fecha,
            g.descripcion as Descripcion,
      g.id_concepto as ConceptoId,
 COALESCE(c.nombre, '') as ConceptoNombre,
cat.id as CategoriaId,
 cat.nombre as CategoriaNombre,
        g.id_proveedor as ProveedorId,
     prov.nombre as ProveedorNombre,
   g.id_persona as PersonaId,
    p.nombre as PersonaNombre,
 g.id_cuenta as CuentaId,
   COALESCE(cta.nombre, '') as CuentaNombre,
           g.id_forma_pago as FormaPagoId,
     COALESCE(fp.nombre, '') as FormaPagoNombre,
    g.id_usuario as UsuarioId
 FROM gastos g
        LEFT JOIN conceptos c ON g.id_concepto = c.id
            LEFT JOIN categorias cat ON c.id_categoria = cat.id
     LEFT JOIN proveedores prov ON g.id_proveedor = prov.id
    LEFT JOIN personas p ON g.id_persona = p.id
  LEFT JOIN cuentas cta ON g.id_cuenta = cta.id
        LEFT JOIN formas_pago fp ON g.id_forma_pago = fp.id
         WHERE g.id = @id";
   }

        /// <summary>
        /// 🔥 Query optimizado para obtener todos los gastos con sus datos relacionados.
    /// </summary>
protected override string BuildGetAllQuery()
   {
    return @"
     SELECT 
       g.id as Id,
 g.importe as Importe,
      g.fecha as Fecha,
       g.descripcion as Descripcion,
      g.id_concepto as ConceptoId,
   COALESCE(c.nombre, '') as ConceptoNombre,
      cat.id as CategoriaId,
 cat.nombre as CategoriaNombre,
 g.id_proveedor as ProveedorId,
    prov.nombre as ProveedorNombre,
 g.id_persona as PersonaId,
    p.nombre as PersonaNombre,
  g.id_cuenta as CuentaId,
             COALESCE(cta.nombre, '') as CuentaNombre,
          g.id_forma_pago as FormaPagoId,
          COALESCE(fp.nombre, '') as FormaPagoNombre,
         g.id_usuario as UsuarioId
  FROM gastos g
LEFT JOIN conceptos c ON g.id_concepto = c.id
    LEFT JOIN categorias cat ON c.id_categoria = cat.id
   LEFT JOIN proveedores prov ON g.id_proveedor = prov.id
  LEFT JOIN personas p ON g.id_persona = p.id
 LEFT JOIN cuentas cta ON g.id_cuenta = cta.id
     LEFT JOIN formas_pago fp ON g.id_forma_pago = fp.id";
        }

        /// <summary>
        /// 🔥 ORDER BY por defecto: ordenar por fecha descendente (más recientes primero).
        /// </summary>
        protected override string GetDefaultOrderBy()
        {
            return "ORDER BY g.fecha DESC, g.id DESC";
        }
}
}