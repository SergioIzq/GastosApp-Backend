namespace AppG.Entidades.BBDD

{
    public class Traspaso : Entidad
    {
        public Traspaso()
        {
        }

        public virtual Cuenta CuentaOrigen { get; set; } = new Cuenta();

        public virtual Cuenta CuentaDestino { get; set; } = new Cuenta();

        public virtual DateTime Fecha { get; set; }

        public virtual decimal Importe { get; set; }

        public virtual string? Descripcion { get; set; }

        public virtual decimal SaldoCuentaOrigen { get; set; }

        public virtual decimal SaldoCuentaDestino { get; set; }

    }

}

