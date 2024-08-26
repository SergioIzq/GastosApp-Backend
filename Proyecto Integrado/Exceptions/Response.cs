namespace AppG.Exceptions
{
    /// <summary>
    /// Representa una respuesta genérica para devolver información a través de una API.
    /// </summary>
    /// <typeparam name="T">El tipo de los elementos en la respuesta.</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Mensaje adicional sobre la operación.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Los elementos de la respuesta.
        /// </summary>
        public T[] Items { get; set; }

        public List<string> Errors {  get; set; }
    }


}
