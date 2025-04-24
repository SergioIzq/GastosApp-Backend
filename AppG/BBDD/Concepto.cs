namespace AppG.Entidades.BBDD

{
    public class Concepto : Entidad
    {
        public Concepto()
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

    }

}

