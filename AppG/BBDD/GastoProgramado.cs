namespace AppG.Entidades.BBDD
{
    public class GastoProgramado : Entidad
    {
        public GastoProgramado()
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

        private DateTime _FechaInicio;
        public virtual DateTime FechaInicio
        {
            get
            {
                return this._FechaInicio;
            }
            set
            {
                if (this._FechaInicio != value)
                {
                    this._FechaInicio = value;
                }
            }
        }
        private DateTime _FechaFin;
        public virtual DateTime FechaFin
        {
            get
            {
                return this._FechaFin;
            }
            set
            {
                if (this._FechaFin != value)
                {
                    this._FechaFin = value;
                }
            }
        }
        public virtual Concepto Concepto { get; set; }
        public virtual Proveedor Proveedor { get; set; }
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

        private string? _Frecuencia;
        public virtual string? Frecuencia
        {
            get
            {
                return this._Frecuencia;
            }
            set
            {
                if (this._Frecuencia != value)
                {
                    this._Frecuencia = value;
                }
            }
        }
        public virtual bool Activo { get; set; }
    }
}
