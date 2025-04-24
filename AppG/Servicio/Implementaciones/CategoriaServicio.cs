using AppG.Controllers;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using OfficeOpenXml;
using System.Diagnostics;

namespace AppG.Servicio
{
    public class CategoriaServicio : BaseServicio<Categoria>, ICategoriaServicio
    {
        public CategoriaServicio(ISessionFactory sessionFactory) : base(sessionFactory) { }


        public override async Task<Categoria> CreateAsync(Categoria entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingCategoria = await session.Query<Categoria>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCategoria != null && existingCategoria.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"La categoría '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                session.Save(entity);
                await transaction.CommitAsync();

                var id = session.GetIdentifier(entity);
                var createdEntity = await session.GetAsync<Categoria>(id);

                return createdEntity;

            }
        }

        public override async Task UpdateAsync(int id, Categoria entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingCategoria = await session.Query<Categoria>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && entity.IdUsuario == c.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCategoria != null && existingCategoria.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"La categoría '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Guardar la entidad y guardar los cambios
                session.Update(entity);
                await transaction.CommitAsync();

            }
        }

        public override Task DeleteAsync(int id)
        {
            return base.DeleteAsync(id);
        }

        public void ExportarDatosExcelAsync(Excel<CategoriaDto> res)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string directorioPath = res.DirPath;

            // Comprobar si la ruta del directorio es válida
            if (!Directory.Exists(directorioPath))
            {
                throw new DirectoryNotFoundException($"El directorio especificado no existe: {directorioPath}");
            }

            // Definir la ruta completa del archivo
            var filePath = Path.Combine(directorioPath, "categorias.xlsx");

            var exportData = new List<dynamic>();

            // Convertir la lista de ingresos a un formato adecuado para Excel
            exportData.AddRange(res.Data.Select(item => new
            {
                Categoria = item?.Nombre ?? string.Empty,
                Descripcion = item?.Descripcion ?? string.Empty,
            }));

            exportData.Add(new
            {
                Categoria = "",
                Descripcion = "",
            });

            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet;

                if (File.Exists(filePath))
                {
                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                        {
                            package.Load(stream);
                        }
                    }
                    catch (FileLoadException ex)
                    {
                        throw new FileLoadException();
                    }
                    worksheet = package.Workbook.Worksheets["Categoria"];

                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add("Categoria");
                    }
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.Add("Categoria");
                }

                worksheet.Cells.Clear();

                // Establecer las cabeceras de las columnas
                worksheet.Cells["A1"].Value = "Categoria";
                worksheet.Cells["B1"].Value = "Descripcion";

                // Cargar los datos manualmente a partir de la fila 2
                var row = 2;
                foreach (var item in exportData.Take(exportData.Count - 1))
                {
                    worksheet.Cells[row, 1].Value = item.Categoria;
                    worksheet.Cells[row, 2].Value = item.Descripcion;
                    row++;
                }

                row++;

                if (worksheet.Dimension != null)
                {
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }

                FileInfo fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);

                // Abrir el archivo en Excel
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
        }


        public class CategoriaDto
        {
            public string Nombre { get; set; }

            public string? Descripcion { get; set; }
        }
    }
}

