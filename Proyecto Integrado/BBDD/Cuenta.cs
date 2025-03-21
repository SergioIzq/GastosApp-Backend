namespace AppG.Entidades.BBDD

{
    public class Cuenta : Entidad
    {
        public Cuenta()
        {

          
        }



        private string _Nombre;
        public virtual string Nombre
        {
            get
            {
                return this._Nombre;
            }
            set
            {
                if (this._Nombre != value)
                {
                    this._Nombre = value;
                }
            }
        }

        private decimal _Saldo;
        public virtual decimal Saldo
        {
            get
            {
                return this._Saldo;
            }
            set
            {
                if (this._Saldo != value)
                {
                    this._Saldo = value;
                }
            }
        }

    }

}

