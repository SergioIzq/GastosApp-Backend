namespace AppG.BBDD.Requests
{
    public class ContactoFormRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}