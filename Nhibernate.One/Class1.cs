using NUnit.Framework;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Mapping;

namespace Nhibernate.One
{
    [TestFixture]
    public class NHibernateTests
    {
        [Test]
        public void CanCreateDatabase()
        {
            var config = Fluently.Configure()
                        .Database(SQLiteConfiguration.Standard                        
                        .ConnectionString($"Data Source=c:\\Development\\testdb.db;Version=3;New=True;"))
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateTests>())
                        .ExposeConfiguration(c =>
                        {                            
                            var se = new SchemaExport(c);                            
                            se.Drop(true, true);
                            se.Create(true, true);
                        })
                        .BuildConfiguration();

            var sessionFactory = config.BuildSessionFactory();
            using (var session = sessionFactory.OpenSession())
            {
                var id = session.Save(new SimpleEntity()
                {
                    Name = "Hello Entity!"
                });

                var entity = session.Get<SimpleEntity>(id);

                Assert.That(entity, Is.Not.Null);
            }
        }

        public class SimpleEntity
        {
            public virtual int Id { get; set; }
            public virtual string Name { get; set; }
        }

        public class SimpleEntityMap : ClassMap<SimpleEntity>
        {
            public SimpleEntityMap()
            {
                Id(x => x.Id, "Id");
                Map(x => x.Name, "Name");
                Table("SimpleEntities");
            }
        }
    }
}
