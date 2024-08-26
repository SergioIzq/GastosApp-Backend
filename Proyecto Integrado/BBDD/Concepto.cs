namespace AppG.Entidades.BBDD

{
    public class Concepto : IEntidad
    {
        public Concepto()
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

        private Categoria _Categoria;
        public virtual Categoria Categoria
        {
            get
            {
                return this._Categoria;
            }
            set
            {
                if (this._Categoria != value)
                {
                    this._Categoria = value;
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

