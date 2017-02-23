using NUnit.Framework;
using System.Linq;

namespace Nhibernate.One
{
    public class QueryGetTests : NHibernateSetup
    {
        /// <summary>
        /// Query will always hit the database
        /// Get will not of the entity is in session or cache
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
                
                transaction.Commit();
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(0));
            Assert.That(_sessionFactory.Statistics.QueryExecutionCount, Is.EqualTo(0));
        }

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
