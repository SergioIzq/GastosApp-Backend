namespace AppG.BBDD.Respuestas
{
    public class ResponseList<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalRecords { get; set; }

        public ResponseList(IEnumerable<T> items, int totalRecords)
        {
            Items = items;
            TotalRecords = totalRecords;
        }
    }
}
