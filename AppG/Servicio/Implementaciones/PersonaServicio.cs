﻿using AppG.Controllers;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using OfficeOpenXml;
using System.Diagnostics;
using System.Linq.Expressions;

namespace AppG.Servicio
{
    public class PersonaServicio : BaseServicio<Persona>, IPersonaServicio
    {
        private readonly IGastoProgramadoServicio _gastoProgramadoServicio;
        public PersonaServicio(ISessionFactory sessionFactory, IGastoProgramadoServicio gastoProgramadoServicio) : base(sessionFactory)
        {
            _gastoProgramadoServicio = gastoProgramadoServicio;
        }


        public override async Task<Persona> CreateAsync(Persona entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                // Verificar si la categoría existe en la base de datos
                var existingPersona = await session.Query<Persona>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingPersona != null && existingPersona.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"La persona '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }


                try
                {
                    // Guardar la entidad y guardar los cambios
                    session.Save(entity);
                    await transaction.CommitAsync();

                    var id = session.GetIdentifier(entity);
                    var createdEntity = await session.GetAsync<Persona>(id);

                    return createdEntity;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                    throw new Exception(ex.Message);

                }

            }
        }

        public override async Task UpdateAsync(int id, Persona entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingPersona = await session.Query<Persona>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingPersona != null && existingPersona.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"La persona '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }


                try
                {
                    // Guardar la entidad y guardar los cambios
                    session.Update(entity);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }

            }
        }

        public override async Task DeleteAsync(int id)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                await EliminarEntidadesRelacionadas<Ingreso>(session, i => i.Persona.Id == id);
                await EliminarEntidadesRelacionadas<Gasto>(session, g => g!.Persona!.Id == id);

                // Eliminar los traspasos asociados a la cuenta como origen
                var gastosProgramados = await session.Query<GastoProgramado>()
                    .Where(c => c!.Persona!.Id == id)
                    .ToListAsync();

                foreach (var gasto in gastosProgramados)
                {
                    await _gastoProgramadoServicio.DeleteAsync(gasto.Id);
                }

                // Eliminar la cuenta
                await base.DeleteAsync(id);

                // Confirmar la transacción
                await transaction.CommitAsync();
            }
        }

        public void ExportarDatosExcelAsync(Excel<PersonaDto> res)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string directorioPath = res.DirPath;

            // Comprobar si la ruta del directorio es válida
            if (!Directory.Exists(directorioPath))
            {
                throw new DirectoryNotFoundException($"El directorio especificado no existe: {directorioPath}");
            }

            // Definir la ruta completa del archivo
            var filePath = Path.Combine(directorioPath, "personas.xlsx");

            var exportData = new List<dynamic>();

            // Convertir la lista de ingresos a un formato adecuado para Excel
            exportData.AddRange(res.Data.Select(item => new
            {
                Nombre = item?.Nombre ?? string.Empty,
            }));

            exportData.Add(new
            {
                Nombre = "",
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
                    catch (FileLoadException)
                    {
                        throw new FileLoadException();
                    }
                    worksheet = package.Workbook.Worksheets["Persona"];

                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add("Persona");
                    }
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.Add("Persona");
                }

                worksheet.Cells.Clear();

                // Establecer las cabeceras de las columnas
                worksheet.Cells["A1"].Value = "Nombre";

                // Cargar los datos manualmente a partir de la fila 2
                var row = 2;
                foreach (var item in exportData.Take(exportData.Count - 1))
                {
                    worksheet.Cells[row, 1].Value = item.Nombre;
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


        public class PersonaDto
        {
            public required string Nombre { get; set; }

        }

        private async Task EliminarEntidadesRelacionadas<T>(NHibernate.ISession session, Expression<Func<T, bool>> predicate) where T : class
        {
            var entidades = await session.Query<T>()
                .Where(predicate)
                .ToListAsync();

            foreach (var entidad in entidades)
            {
                session.Delete(entidad);
            }
        }
    }
}

