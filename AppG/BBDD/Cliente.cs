namespace AppG.Entidades.BBDD

{
    public class Cliente : Entidad
    {
        public Cliente()
        {
        }

        public virtual string Nombre { get; set; } = string.Empty;
    }

}

