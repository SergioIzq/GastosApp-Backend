using System.Collections.Generic;

namespace AppG.Controllers
{
    public class ResponseOne<T>
    {
        public T Item { get; set; }
        public string message { get; set; }

        public ResponseOne(T item, string message)
        {
            Item = item;
            message = message;
        }
    }
}
