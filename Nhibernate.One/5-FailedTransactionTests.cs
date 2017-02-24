using System;
using NUnit.Framework;
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
                .WithDefaultConfiguration()
                .Build();
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
        public void FailedTransaction()
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
                catch (Exception exception)
                {
                    transaction.Rollback();
                    Assert.That(exception.Message, Is.EqualTo("Something went wrong"));
                }
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(0));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(0));
        }

        [Test]
        public void FailedTransactionWithMultipleSaves()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var entityOne = new SimpleEntity()
                    {
                        Name = "Successfully Save"
                    };

                    session.Save(entityOne);

                    var entityTwo = new SimpleEntity()
                    {
                        Name = "FailTransaction"
                    };

                    session.Save(entityTwo);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    Assert.That(exception.Message, Is.EqualTo("Something went wrong"));
                }
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(0));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(0));
        }

        [Test]
        public void FailedTransactionWithUpdateSaves()
        {
            var id = 0;
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                {
                    id = (int)session.Save(new SimpleEntity()
                    {
                        Name = "EntityOne"
                    });

                    transaction.Commit();
                }
            }

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var entityOne = session.Load<SimpleEntity>(id);
                    entityOne.OtherField = "Other Field";

                    var entityTwo = new SimpleEntity
                    {
                        Name = "FailTransaction"
                    };

                    session.Save(entityTwo);
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    Assert.That(exception.Message, Is.EqualTo("Something went wrong"));
                }
            }

            Assert.That(_sessionFactory.Statistics.EntityInsertCount, Is.EqualTo(1));
            Assert.That(_sessionFactory.Statistics.EntityUpdateCount, Is.EqualTo(0));
            Assert.That(_sessionFactory.Statistics.EntityLoadCount, Is.EqualTo(1));
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (((SimpleEntity)entity).Name == "FailTransaction")
                throw new Exception("Something went wrong");
            else
                return base.OnSave(entity, id, state, propertyNames, types);
        }
    }
}
