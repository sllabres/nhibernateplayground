using NHibernate;
using NUnit.Framework;
using System.Linq;

namespace Nhibernate.One
{
    /// http://stackoverflow.com/questions/2125668/difference-between-gett-and-loadt
    /// https://ayende.com/blog/3988/nhibernate-the-difference-between-get-load-and-querying-by-id
    public class QueryGetTests
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
        /// Get hits the database or session cache to retrieve the entity data. If the entity exists it is returned, otherwise null will be returned.This is the safest way to determine whether an entity with a certain ID exists or not. If you're not sure what to use, use Get.
        /// </summary>
        [Test]
        public void Get()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var id = (int)session.Save(new SimpleEntity()
                {
                    Name = "Hello Entity!"
                });

                var entity = session.Get<SimpleEntity>(id);

                Assert.That(session.Contains(entity), Is.True);

                transaction.Commit();
            }



            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(0));
            Assert.That(_sessionFactory.Statistics.QueryExecutionCount, Is.EqualTo(0));
        }

        /// <summary>        
        /// Load should be used when you know for sure that an entity with a certain ID exists. The call does not result in a database hit (and thus can be optimized away by NHibernate in certain cases). Beware of the exception that may be raised when the object is accessed if the entity instance doesn't exist in the DB.        
        /// </summary>
        [Test]
        public void Load()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var id = (int)session.Save(new SimpleEntity()
                {
                    Name = "Hello Entity!"
                });

                var entity = session.Load<SimpleEntity>(id);

                Assert.That(session.Contains(entity), Is.True);
                transaction.Commit();
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(0));
            Assert.That(_sessionFactory.Statistics.QueryExecutionCount, Is.EqualTo(0));
        }

        /// Query will always hit the database
        [Test]
        public void Query()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var id = (int)session.Save(new SimpleEntity()
                {
                    Name = "Hello Entity!"
                });

                var queryEntity = session.QueryOver<SimpleEntity>()
                    .Where(e => e.Id == id).List().First();

                Assert.That(queryEntity.Name, Is.EqualTo("Hello Entity!"));
                transaction.Commit();
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(0));
            Assert.That(_sessionFactory.Statistics.QueryExecutionCount, Is.EqualTo(1));
        }
    }
}
