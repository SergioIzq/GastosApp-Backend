namespace AppG.Entidades.BBDD

{
    public abstract class Entidad
    {
        public Entidad()
        {
            Id = 0;
            IdUsuario = 0;
            FechaCreacion = DateTime.Now;
        }

        public virtual int Id { get; set; }

        public virtual int IdUsuario { get; set; }

        public virtual DateTime FechaCreacion { get; set; }


    }

}

