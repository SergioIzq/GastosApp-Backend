namespace AppG.Entidades.BBDD

{
    public class Cuenta : IEntidad
    {
        public Cuenta()
        {

          
        }


        private int _Id;
        public virtual int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                if (this._Id != value)
                {
                    this._Id = value;
                }
            }
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

        private int _IdUsuario;
        public virtual int IdUsuario
        {
            get
            {
                return this._IdUsuario;
            }
            set
            {
                if (this._IdUsuario != value)
                {
                    this._IdUsuario = value;
                }
            }
        }

    }

}

