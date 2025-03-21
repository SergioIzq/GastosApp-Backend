namespace AppG.Entidades.BBDD

{
    public class Traspaso : Entidad
    {
        public Traspaso()
        {


        }
        private Cuenta _CuentaOrigen;
        public virtual Cuenta CuentaOrigen
        {
            get
            {
                return this._CuentaOrigen;
            }
            set
            {
                if (this._CuentaOrigen != value)
                {
                    this._CuentaOrigen = value;
                }
            }
        }

        private Cuenta _CuentaDestino;
        public virtual Cuenta CuentaDestino
        {
            get
            {
                return this._CuentaDestino;
            }
            set
            {
                if (this._CuentaDestino != value)
                {
                    this._CuentaDestino = value;
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
        private decimal _Importe;
        public virtual decimal Importe
        {
            get
            {
                return this._Importe;
            }
            set
            {
                if (this._Importe != value)
                {
                    this._Importe = value;
                }
            }
        }

        private string _Descripcion;
        public virtual string Descripcion
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
        private decimal _SaldoCuentaOrigen;
        public virtual decimal SaldoCuentaOrigen
        {
            get
            {
                return this._SaldoCuentaOrigen;
            }
            set
            {
                if (this._SaldoCuentaOrigen != value)
                {
                    this._SaldoCuentaOrigen = value;
                }
            }
        }
        private decimal _SaldoCuentaDestino;
        public virtual decimal SaldoCuentaDestino
        {
            get
            {
                return this._SaldoCuentaDestino;
            }
            set
            {
                if (this._SaldoCuentaDestino != value)
                {
                    this._SaldoCuentaDestino = value;
                }
            }
        }

    }

}

