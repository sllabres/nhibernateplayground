using System;
using NUnit.Framework;
using System.Linq;
using NHibernate;
using NHibernate.Type;

namespace Nhibernate.One
{
    public class FailedTransactionTests : EmptyInterceptor
    {
        private ISessionFactory _sessionFactory;

        [SetUp]
        public void SetUp()
        {
            _sessionFactory = new NHibernateSessionFactoryBuilder()
                .WithInterceptor(this)
                .WithDefaultConfiguration().Build();
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
        public void NHibernateFailedTransaction()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var entity = new SimpleEntity()
                    {
                        Name = "FailedTransaction"
                    };

                    session.Save(entity);
                    transaction.Commit();
                }
                catch(Exception exception)
                {
                    transaction.Rollback();
                    Assert.That(exception.Message, Is.EqualTo("Something went wrong"));
                }
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(0));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(0));
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            throw new Exception("Something went wrong");
        }
    }
}
