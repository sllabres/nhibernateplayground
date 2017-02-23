using NUnit.Framework;
using System.Linq;

namespace Nhibernate.One
{
    public class RefreshTests : NHibernateSetup
    {
        /// <summary>
        /// Refresh incurs an additional database hit
        /// When saving the Id is returned by nhibernate
        /// </summary>
        [Test]
        public void SaveAndRefresh()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var entity = new SimpleEntity()
                {
                    Name = "Hello Entity!"
                };

                session.Save(entity);
                session.Refresh(entity);                
                transaction.Commit();                
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(1));
        }

        [Test]
        public void SaveOnly()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var entity = new SimpleEntity()
                {
                    Name = "Hello Entity!"
                };

                var id = (int)session.Save(entity);                
                transaction.Commit();                
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(0));
        }
    }
}
