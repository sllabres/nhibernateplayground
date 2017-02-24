using NUnit.Framework;
using NHibernate;

namespace Nhibernate.One
{
    public class UpdatingTests
    {
        private ISessionFactory _sessionFactory;

        [SetUp]
        public void SetUp()
        {
            _sessionFactory = new NHibernateSessionFactoryBuilder().WithDefaultConfiguration().Build();
        }

        [TearDown]
        public void TearDown()
        {
            _sessionFactory.Dispose();
        }

        /// <summary>
        /// Entities are attached to session and nhibernate tracks changes
        /// Calling save isn't needed on attached objects
        /// Update happens on flush dirty, at the end of the transaction on commit.        
        /// Calling flush isn't needed
        /// </summary>
        [Test]
        public void UpdateLoad()
        {
            int id = 0;
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                id = (int)session.Save(new SimpleEntity()
                {
                    Name = "Initial Name",
                    OtherField = "Initial Field"
                });

                transaction.Commit();
            }

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var entity = session.Load<SimpleEntity>(id);

                entity.Name = "Changed Name";
                transaction.Commit();
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityUpdateCount, Is.EqualTo(1));
        }

        [Test]
        public void UpdateGet()
        {
            int id = 0;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                {
                    id = (int)session.Save(new SimpleEntity()
                    {
                        Name = "FirstEntity"
                    });

                    transaction.Commit();
                }
            }

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                {
                    var entity = session.Get<SimpleEntity>(id);
                    entity.Name = "NewName";

                    session.Save(new SimpleEntity()
                    {
                        Name = "SecondEntity"
                    });

                    var secondEntityGet = session.Get<SimpleEntity>(id);

                    transaction.Commit();
                }
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(2));
            Assert.That(_sessionFactory.Statistics.EntityUpdateCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(1));
        }
    }
}
