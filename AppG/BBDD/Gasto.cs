using System;
using System.ComponentModel.DataAnnotations;

namespace AppG.Entidades.BBDD
{
    public class Gasto : Entidad
    {
        public Gasto()
        {
        }

        public virtual decimal Monto { get; set; }
        public virtual DateTime Fecha { get; set; } = new DateTime();
        public virtual Concepto Concepto { get; set; } = new Concepto();
        public virtual Proveedor Proveedor { get; set; } = new Proveedor();
        public virtual Persona Persona { get; set; } = new Persona();
        public virtual Cuenta Cuenta { get; set; } = new Cuenta();
        public virtual FormaPago FormaPago { get; set; } = new FormaPago();
        public virtual string? Descripcion { get; set; }
    }
}
