namespace AppG.Entidades.BBDD

{
    public class TraspasoProgramado : Entidad
    {
        public TraspasoProgramado()
        {
        }

        public virtual Cuenta CuentaOrigen { get; set; } = new Cuenta();
        public virtual Cuenta CuentaDestino { get; set; } = new Cuenta();
        public virtual decimal Importe { get; set; }
        public virtual string? Descripcion { get; set; }
        public virtual decimal SaldoCuentaOrigen { get; set; }
        public virtual decimal SaldoCuentaDestino { get; set; }
        public virtual bool Activo { get; set; }
        public virtual string HangfireJobId { get; set; } = string.Empty;
        public virtual string Frecuencia { get; set; } = string.Empty;
        public virtual DateTime FechaEjecucion { get; set; }
    }

}

