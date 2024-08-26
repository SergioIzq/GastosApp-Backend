namespace AppG.Entidades.BBDD

{
    public class Categoria : IEntidad
    {
        public Categoria()
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

