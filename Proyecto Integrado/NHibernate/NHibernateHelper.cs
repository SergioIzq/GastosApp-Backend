using AppG.Entidades.BBDD;
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
            configuration.Configure(); // Asume que el archivo hibernate.cfg.xml está en la raíz del proyecto

            // Leer la cadena de conexión de la variable de entorno
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La variable de entorno 'DB_CONNECTION_STRING' no está configurada.");
            }

            // Establecer la cadena de conexión programáticamente
            configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);

            // Añadir ensamblaje
            configuration.AddAssembly(typeof(Categoria).Assembly);

            _sessionFactory = configuration.BuildSessionFactory();
        }
        return _sessionFactory;
    }
}
