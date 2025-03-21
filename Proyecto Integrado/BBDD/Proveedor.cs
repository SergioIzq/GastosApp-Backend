namespace AppG.Entidades.BBDD

{
    public class Proveedor : Entidad
    {
        public Proveedor()
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
    }

}

