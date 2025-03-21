namespace AppG.Entidades.BBDD

{
    public class Ingreso : Entidad
    {
        public Ingreso()
        {

          
        }

        private decimal _Monto;
        public virtual decimal Monto
        {
            get
            {
                return this._Monto;
            }
            set
            {
                if (this._Monto != value)
                {
                    this._Monto = value;
                }
            }
        }

        private DateTime _Fecha;
        public virtual DateTime Fecha
        {
            get
            {
                return this._Fecha;
            }
            set
            {
                if (this._Fecha != value)
                {
                    this._Fecha = value;
                }
            }
        }

        public virtual Concepto Concepto { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Persona Persona { get; set; }
        public virtual Cuenta Cuenta { get; set; }
        public virtual FormaPago FormaPago { get; set; }


        private string? _Descripcion;
        public virtual string? Descripcion
        {
            get
            {
                return this._Descripcion;
            }
            set
            {
                if (this._Descripcion != value)
                {
                    this._Descripcion = value;
                }
            }
        }

    }

}

