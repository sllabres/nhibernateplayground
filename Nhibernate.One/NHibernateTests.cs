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
                            .ConnectionString($"Data Source=c:\\SourceCode\\Learning\\nhibernateplayground\\testdb.db;Version=3;New=True;"))
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
        public void SaveAndRefresh()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var entity = new SimpleEntity()
                {
                    Name = "Hello Entity!"
                };

                session.Save(entity);
                session.Refresh(entity);
                Assert.That(entity.Id, Is.Not.Null);
                Assert.That(entity, Is.Not.Null);
            }

            using (var session = _sessionFactory.OpenSession())
            {
                var entity = new SimpleEntity()
                {
                    Name = "Hello Entity!"
                };

                var id = (int)session.Save(entity);
                Assert.That(id, Is.Not.Null);
                Assert.That(entity, Is.Not.Null);
            }
        }

        [Test]
        public void QueryVsGet()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var id = (int)session.Save(new SimpleEntity()
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

        [Test]
        public void LoadEntityAndMakeChanges()
        {
            int id = 0;
            using (var session = _sessionFactory.OpenSession())
            {
                id = (int)session.Save(new SimpleEntity()
                {
                    Name = "Before Load!",
                    OtherField = "Other Field!"
                });

                var entity = session.Get<SimpleEntity>(id);
                Assert.That(entity, Is.Not.Null);
            }

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var entity = session.Load<SimpleEntity>(id);
                entity.Name = "Loaded Entity!";

                var otherEntity = session.Get<SimpleEntity>(999);

                session.Save(entity);
                transaction.Commit();
            }

            using (var session = _sessionFactory.OpenSession())
            {
                var entity = session.Get<SimpleEntity>(id);
                Assert.That(entity.Name, Is.EqualTo("Loaded Entity!"));
                Assert.That(entity.OtherField, Is.EqualTo("Other Field!"));
            }
        }

        public class SimpleEntity
        {
            public virtual int Id { get; set; }
            public virtual string Name { get; set; }
            public virtual string OtherField { get; set; }
        }

        public class SimpleEntityMap : ClassMap<SimpleEntity>
        {
            public SimpleEntityMap()
            {
                Id(x => x.Id, "Id");
                Map(x => x.Name, "Name");
                Map(x => x.OtherField, "OtherField");
                Table("SimpleEntities");
            }
        }
    }
}
