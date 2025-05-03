namespace AppG.BBDD
{
    public class ExportExcelRequest
    {
        public ResumenDatos Datos { get; set; } = new ResumenDatos();
        public string DirPath { get; set; } = string.Empty;
    }
}
