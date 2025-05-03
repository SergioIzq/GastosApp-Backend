namespace AppG.Entidades.BBDD
{
    public class GastoProgramado : Entidad
    {
        public GastoProgramado()
        {
        }        
        public virtual decimal Monto { get; set; }
        public virtual int DiaEjecucion { get; set; }
        public virtual Concepto Concepto { get; set; } = new Concepto();
        public virtual Proveedor Proveedor { get; set; } = new Proveedor();
        public virtual Persona Persona { get; set; } = new Persona();
        public virtual Cuenta Cuenta { get; set; } = new Cuenta();
        public virtual FormaPago FormaPago { get; set; } = new FormaPago();
        public virtual string? Descripcion{get; set;}
        public virtual bool Activo { get; set; }
        public virtual bool AjustarAUltimoDia { get; set; }
        public virtual string? HangfireJobId { get; set; }

    }
}
