namespace AppG.Entidades.BBDD

{
    public class Cliente : Entidad
    {
        public Cliente()
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

