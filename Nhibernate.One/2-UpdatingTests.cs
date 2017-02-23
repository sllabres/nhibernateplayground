using NUnit.Framework;
using System.Linq;

namespace Nhibernate.One
{
    public class UpdatingTests : NHibernateSetup
    {

        /// <summary>
        /// Update happens on flush dirty, at the end of the transaction.
        /// Calling flush isn't needed
        /// </summary>
        [Test]
        public void CanUpdateEntity()
        {
            int _lastId = 0;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                {
                    var id = session.Save(new SimpleEntity()
                    {
                        Name = "FirstEntity"
                    });

                    _lastId = (int)id;

                    transaction.Commit();
                }
            }

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                {
                    var entity = session.Get<SimpleEntity>(_lastId);
                    entity.Name = "NewName";

                    session.Save(new SimpleEntity()
                    {
                        Name = "SecondEntity"
                    });                    

                    var secondEntityGet = session.Get<SimpleEntity>(_lastId);

                    transaction.Commit();
                }
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(2));
            Assert.That(_sessionFactory.Statistics.EntityUpdateCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(1));
        }
    }
}
