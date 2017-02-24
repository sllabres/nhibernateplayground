using NUnit.Framework;
using NHibernate;

namespace Nhibernate.One
{
    public class InsertingTests
    {
        private ISessionFactory _sessionFactory;

        [SetUp]
        public void SetUp()
        {
            _sessionFactory = new NHibernateSessionFactoryBuilder()
                .WithDefaultConfiguration()
                .Build();
        }

        [TearDown]
        public void TearDown()
        {
            _sessionFactory.Dispose();
        }
        /// <summary>
        /// Simple demonstration of inserting data using a transaction        
        /// Good practise to always use a transaction, even for reading data
        /// This is not the same as a database transaction
        /// https://ayende.com/blog/3775/nh-prof-alerts-use-of-implicit-transactions-is-discouraged
        /// </summary>
        [Test]
        public void CanInsertEntity()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                {
                    var id = session.Save(new SimpleEntity()
                    {
                        Name = "Hello Entity!"
                    });

                    var entity = session.Get<SimpleEntity>(id);
                    transaction.Commit();
                    Assert.That(entity, Is.Not.Null);
                    Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));
                    Assert.That(_sessionFactory.Statistics.QueryExecutionCount, Is.EqualTo(0));
                }
            }
        }
    }
}
