using System.Collections.Generic;

namespace AppG.Controllers
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
