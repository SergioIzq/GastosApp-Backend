using AppG.Entidades.BBDD;
using AppG.Servicio;
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

            // Establecer la cadena de conexión programáticamente
            configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);

            // Añadir ensamblaje
            configuration.AddAssembly(typeof(Categoria).Assembly);

            _sessionFactory = configuration.BuildSessionFactory();
        }
        return _sessionFactory;
    }
}
