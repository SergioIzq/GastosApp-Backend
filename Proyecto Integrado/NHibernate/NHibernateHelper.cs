using AppG.Entidades.BBDD;
using NHibernate;

public class NHibernateHelper
{
    private static ISessionFactory _sessionFactory;

    public static ISessionFactory GetSessionFactory()
    {
        if (_sessionFactory == null)
        {
            // Crear una nueva configuración de NHibernate
            NHibernate.Cfg.Configuration configuration = new NHibernate.Cfg.Configuration();

            configuration.Configure();

            string connectionString = Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no está configurada en las variables de entorno.");
            }

            configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);

            configuration.AddAssembly(typeof(Categoria).Assembly);

            _sessionFactory = configuration.BuildSessionFactory();
        }
        return _sessionFactory;
    }
}
