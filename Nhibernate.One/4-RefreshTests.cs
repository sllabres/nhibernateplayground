using NUnit.Framework;
using System.Linq;
using NHibernate;

namespace Nhibernate.One
{
    public class RefreshTests
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
