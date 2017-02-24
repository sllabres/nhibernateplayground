using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using NHibernate.Cfg;

namespace Nhibernate.One
{
    public sealed class NHibernateSessionFactoryBuilder
    {
        private IInterceptor _interceptor;
        private FluentConfiguration _configuration;

        public NHibernateSessionFactoryBuilder()
        {
            _interceptor = new TestInterceptor();
        }

        public NHibernateSessionFactoryBuilder WithInterceptor(IInterceptor interceptor)
        {
            _interceptor = interceptor;
            return this;
        }

        public NHibernateSessionFactoryBuilder WithDefaultConfiguration()
        {
            var databaseLocation = $"{Path.GetDirectoryName(new System.Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath)}\\testdb.db";
            _configuration = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard
                    .ConnectionString($"Data Source={databaseLocation};Version=3;New=True;"))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateSessionFactoryBuilder>());

            return this;
        }

        private void ExposeConfiguration(Configuration c)
        {
            c.SetProperty("generate_statistics", "true");
            var se = new SchemaExport(c);
            se.Drop(true, true);
            se.Create(true, true);
            c.SetInterceptor(_interceptor);
        }

        public ISessionFactory Build()
        {
            return _configuration
                        .ExposeConfiguration(ExposeConfiguration)
                        .BuildConfiguration()
                        .BuildSessionFactory();
        }
    }
}
