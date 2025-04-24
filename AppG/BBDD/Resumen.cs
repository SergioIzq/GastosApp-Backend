namespace AppG.Entidades.BBDD

{
    public class Resumen
    {
        public Resumen()
        {


        }

        private string _IngresosTotales;
        public virtual string IngresosTotales
        {
            get
            {
                return this._IngresosTotales;
            }
            set
            {
                if (this._IngresosTotales != value)
                {
                    this._IngresosTotales = value;
                }
            }
        }

        private string _GastosTotales;
        public virtual string GastosTotales
        {
            get
            {
                return this._GastosTotales;
            }
            set
            {
                if (this._GastosTotales != value)
                {
                    this._GastosTotales = value;
                }
            }
        }

        private DateTime _Periodo;
        public virtual DateTime Periodo
        {
            get
            {
                return this._Periodo;
            }
            set
            {
                if (this._Periodo != value)
                {
                    this._Periodo = value;
                }
            }
        }
    }

}

