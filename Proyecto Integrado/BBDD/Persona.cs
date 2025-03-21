namespace AppG.Entidades.BBDD

{
    public class Persona: Entidad
    {
        public Persona()
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

