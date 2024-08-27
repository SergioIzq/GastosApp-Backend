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


            // Añadir ensamblaje
            configuration.AddAssembly(typeof(Categoria).Assembly);

            _sessionFactory = configuration.BuildSessionFactory();
        }
        return _sessionFactory;
    }
}
