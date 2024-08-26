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
            

            NHibernate.Cfg.Configuration configuration = new NHibernate.Cfg.Configuration();


            configuration.AddAssembly(typeof(Categoria).Assembly);

            _sessionFactory = configuration.BuildSessionFactory();
        }
        return _sessionFactory;
    }
}
