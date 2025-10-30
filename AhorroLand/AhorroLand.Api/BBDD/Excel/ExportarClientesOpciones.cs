namespace AppG.BBDD.Excel
{
    public class ExportarOpciones
    {
        public string NombreArchivo { get; set; } = "exportacion";
        public string Origen { get; set; } = "bbdd"; // "bbdd" o "tabla"
        public int? Pagina { get; set; }
        public int? Tamano { get; set; }
        public List<string> Columnas { get; set; } = new();
        public int IdUsuario { get; set; }
    }

}
