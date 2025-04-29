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

        private int _DiaEjecucion;
        public virtual int DiaEjecucion
        {
            get
            {
                return this._DiaEjecucion;
            }
            set
            {
                if (this._DiaEjecucion != value)
                {
                    this._DiaEjecucion = value;
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
        public virtual bool Activo { get; set; }
        public virtual bool AjustarAUltimoDia { get; set; }
    }
}
