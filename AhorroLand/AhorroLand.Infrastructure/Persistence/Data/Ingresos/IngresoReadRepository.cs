using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Ingresos
{
    public class IngresoReadRepository : AbsReadRepository<Ingreso, IngresoDto>, IIngresoReadRepository
    {
        public IngresoReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "ingresos")
        {
        }

        /// <summary>
        /// 🔥 Alias de la tabla principal para usar en JOINs.
        /// </summary>
        protected override string GetTableAlias()
        {
            return "i";
        }

        /// <summary>
        /// 🔥 Query de conteo que usa el alias correcto.
        /// </summary>
        protected override string BuildCountQuery()
        {
            return "SELECT COUNT(*) FROM ingresos i";
        }

        /// <summary>
        /// 🔥 Query optimizado para obtener un ingreso con todos sus datos relacionados.
        /// </summary>
        protected override string BuildGetByIdQuery()
        {
            return @"
                SELECT 
          i.id as Id,
       i.importe as Importe,
           i.fecha as Fecha,
  i.descripcion as Descripcion,
         i.id_concepto as ConceptoId,
         COALESCE(c.nombre, '') as ConceptoNombre,
     cat.id as CategoriaId,
    cat.nombre as CategoriaNombre,
      i.id_cliente as ClienteId,
          COALESCE(cli.nombre, '') as ClienteNombre,
          i.id_persona as PersonaId,
          COALESCE(p.nombre, '') as PersonaNombre,
                i.id_cuenta as CuentaId,
 COALESCE(cta.nombre, '') as CuentaNombre,
      i.id_forma_pago as FormaPagoId,
COALESCE(fp.nombre, '') as FormaPagoNombre,
        i.id_usuario as UsuarioId
    FROM ingresos i
            LEFT JOIN conceptos c ON i.id_concepto = c.id
  LEFT JOIN categorias cat ON c.id_categoria = cat.id
         LEFT JOIN clientes cli ON i.id_cliente = cli.id
        LEFT JOIN personas p ON i.id_persona = p.id
  LEFT JOIN cuentas cta ON i.id_cuenta = cta.id
      LEFT JOIN formas_pago fp ON i.id_forma_pago = fp.id
    WHERE i.id = @id";
        }

        /// <summary>
        /// 🔥 Query optimizado para obtener todos los ingresos con sus datos relacionados.
        /// </summary>
        protected override string BuildGetAllQuery()
        {
       return @"
   SELECT 
       i.id as Id,
         i.importe as Importe,
            i.fecha as Fecha,
             i.descripcion as Descripcion,
          i.id_concepto as ConceptoId,
       COALESCE(c.nombre, '') as ConceptoNombre,
    cat.id as CategoriaId,
    cat.nombre as CategoriaNombre,
i.id_cliente as ClienteId,
     COALESCE(cli.nombre, '') as ClienteNombre,
   i.id_persona as PersonaId,
       COALESCE(p.nombre, '') as PersonaNombre,
i.id_cuenta as CuentaId,
 COALESCE(cta.nombre, '') as CuentaNombre,
i.id_forma_pago as FormaPagoId,
        COALESCE(fp.nombre, '') as FormaPagoNombre,
     i.id_usuario as UsuarioId
    FROM ingresos i
       LEFT JOIN conceptos c ON i.id_concepto = c.id
      LEFT JOIN categorias cat ON c.id_categoria = cat.id
  LEFT JOIN clientes cli ON i.id_cliente = cli.id
     LEFT JOIN personas p ON i.id_persona = p.id
       LEFT JOIN cuentas cta ON i.id_cuenta = cta.id
LEFT JOIN formas_pago fp ON i.id_forma_pago = fp.id";
   }

  /// <summary>
        /// 🔥 ORDER BY por defecto: ordenar por fecha descendente (más recientes primero).
     /// </summary>
  protected override string GetDefaultOrderBy()
  {
            return "ORDER BY i.fecha DESC, i.id DESC";
        }
    }
}