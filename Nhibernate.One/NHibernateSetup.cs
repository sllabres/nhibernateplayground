using System.IO;
using System.Reflection;
using NUnit.Framework;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using NHibernate;

namespace Nhibernate.One
{
    public abstract class NHibernateSetup
    {
        protected ISessionFactory _sessionFactory;

        [SetUp]
        public void SetUp()
        {
            var databaseLocation = $"{Path.GetDirectoryName(new System.Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath)}\\testdb.db";
            var config = Fluently.Configure()
                        .Database(SQLiteConfiguration.Standard
                            .ConnectionString($"Data Source={databaseLocation};Version=3;New=True;"))
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateSetup>())
                        .ExposeConfiguration(c =>
                        {
                            c.SetProperty("generate_statistics", "true");
                            var se = new SchemaExport(c);
                            se.Drop(true, true);
                            se.Create(true, true);
                            c.SetInterceptor(new TestInterceptor());
                        })                        
                        .BuildConfiguration();

            _sessionFactory = config.BuildSessionFactory();
        }

        [TearDown]
        public void TearDown()
        {
            _sessionFactory.Dispose();
        }
    }
}
