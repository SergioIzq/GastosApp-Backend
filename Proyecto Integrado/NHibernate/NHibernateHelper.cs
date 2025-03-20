using AppG.Entidades.BBDD;
using Microsoft.AspNet.Identity;
using NHibernate;

public class NHibernateHelper
{
    private static ISessionFactory _sessionFactory;

    public static ISessionFactory GetSessionFactory()
    {
        if (_sessionFactory == null)
        {
            var configuration = new NHibernate.Cfg.Configuration();

            // Configuración de NHibernate desde el archivo XML
            configuration.Configure(); // Asegúrate de que este archivo esté correctamente configurado

            configuration.Configure();

            string connectionString = Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no está configurada en las variables de entorno.");
            }

            configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);

            // Añadir ensamblaje
            configuration.AddAssembly(typeof(Categoria).Assembly);

            _sessionFactory = configuration.BuildSessionFactory();
        }
        return _sessionFactory;
    }
}
