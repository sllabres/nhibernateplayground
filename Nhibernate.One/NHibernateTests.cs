using NUnit.Framework;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Mapping;
using NHibernate;
using System.Linq;

namespace Nhibernate.One
{
    [TestFixture]
    public class NHibernateTests
    {
        ISessionFactory _sessionFactory;

        [SetUp]
        public void SetUp()
        {
            var config = Fluently.Configure()
                        .Database(SQLiteConfiguration.Standard
                            .ConnectionString($"Data Source=c:\\Development\\testdb.db;Version=3;New=True;")
                            .ShowSql())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateTests>())
                        .ExposeConfiguration(c =>
                        {
                            var se = new SchemaExport(c);
                            se.Drop(true, true);
                            se.Create(true, true);
                        })
                        .BuildConfiguration();

            _sessionFactory = config.BuildSessionFactory();
        }

        [TearDown]
        public void TearDown()
        {
            _sessionFactory.Dispose();
        }

        [Test]
        public void CanInsertEntity()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var id = session.Save(new SimpleEntity()
                {
                    Name = "Hello Entity!"
                });

                var entity = session.Get<SimpleEntity>(id);
                Assert.That(entity, Is.Not.Null);
            }
        }

        [Test]
        public void QueryVsGet()
        {
            int id = 0;
            using (var session = _sessionFactory.OpenSession())
            {
                id = (int)session.Save(new SimpleEntity()
                {
                    Name = "Hello Entity!"
                });

                var entity = session.Get<SimpleEntity>(id);

                var queryEntity = session.QueryOver<SimpleEntity>()
                    .Where(e => e.Id == id).List().First();

                Assert.That(entity.Name, Is.EqualTo("Hello Entity!"));
                Assert.That(queryEntity.Name, Is.EqualTo("Hello Entity!"));
                Assert.That(id, Is.Not.Null);
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
