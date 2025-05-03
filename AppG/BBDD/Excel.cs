namespace AppG.Entidades.BBDD
{
    public class Excel<T> where T : class
    {
        public List<T> Data { get; set; } = new List<T>();
        public string DirPath { get; set; } = string.Empty;
    }
}
