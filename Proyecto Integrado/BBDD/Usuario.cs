namespace AppG.Entidades.BBDD

{
    public class Usuario
    {
        public Usuario()
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


        private string _Correo;
        public virtual string Correo
        {
            get
            {
                return this._Correo;
            }
            set
            {
                if (this._Correo != value)
                {
                    this._Correo = value;
                }
            }
        }

        private string _Contrasena;
        public virtual string Contrasena
        {
            get
            {
                return this._Contrasena;
            }
            set
            {
                if (this._Contrasena != value)
                {
                    this._Contrasena = value;
                }
            }
        }
    }

}

