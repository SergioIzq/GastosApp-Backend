namespace AppG.Entidades.BBDD

{
    public class Categoria : Entidad
    {
        public Categoria()
        {
        }

        public virtual string Nombre { get; set; } = string.Empty;

        public virtual string? Descripcion { get; set; }
    }

}

