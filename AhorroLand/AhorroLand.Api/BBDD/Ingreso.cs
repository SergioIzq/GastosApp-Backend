namespace AppG.Entidades.BBDD

{
    public class Ingreso : Entidad
    {
        public Ingreso()
        {


        }
        public virtual decimal Importe { get; set; }
        public virtual DateTime Fecha { get; set; }

        public virtual Concepto Concepto { get; set; } = new Concepto();
        public virtual Cliente Cliente { get; set; } = new Cliente();
        public virtual Persona Persona { get; set; } = new Persona();
        public virtual Cuenta Cuenta { get; set; } = new Cuenta();
        public virtual FormaPago FormaPago { get; set; } = new FormaPago();
        public virtual string? Descripcion { get; set; }

    }

}

