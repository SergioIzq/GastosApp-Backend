﻿namespace AppG.BBDD.Respuestas
{
    public class ResponseOne<T>
    {
        public T Item { get; set; }
        public string message { get; set; }

        public ResponseOne(T item, string message)
        {
            Item = item;
            this.message = message;
        }
    }
}
