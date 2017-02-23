using NUnit.Framework;
using System.Linq;

namespace Nhibernate.One
{
    public class LoadTests : NHibernateSetup
    {

        /// <summary>
        /// http://stackoverflow.com/questions/2125668/difference-between-gett-and-loadt
        /// https://ayende.com/blog/3988/nhibernate-the-difference-between-get-load-and-querying-by-id
        /// Load should be used when you know for sure that an entity with a certain ID exists. The call does not result in a database hit (and thus can be optimized away by NHibernate in certain cases). Beware of the exception that may be raised when the object is accessed if the entity instance doesn't exist in the DB.
        /// Get hits the database or session cache to retrieve the entity data. If the entity exists it is returned, otherwise null will be returned.This is the safest way to determine whether an entity with a certain ID exists or not. If you're not sure what to use, use Get.
        /// </summary>
        [Test]
        public void LoadEntityAndMakeChanges()
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
                session.Save(entity);
                transaction.Commit();
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));            
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityUpdateCount, Is.EqualTo(1));
        }
    }
}
