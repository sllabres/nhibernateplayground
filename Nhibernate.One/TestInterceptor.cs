using NHibernate;
using NHibernate.Type;
using NLog;


namespace Nhibernate.One
{
    public class TestInterceptor : EmptyInterceptor
    {
        private Logger log = LogManager.GetLogger("NHibernate.SQL");

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            log.Debug($"OnFlushDirty: {(entity as SimpleEntity).Name} - {id}");
            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public override void AfterTransactionBegin(ITransaction tx)
        {
            log.Debug("AfterTransactionBegin");
            base.AfterTransactionBegin(tx);
        }

        public override void BeforeTransactionCompletion(ITransaction tx)
        {
            log.Debug("BeforeTransactionCompletion");
            base.BeforeTransactionCompletion(tx);
        }

        public override void AfterTransactionCompletion(ITransaction tx)
        {
            log.Debug("AfterTransactionCompletion");
            base.AfterTransactionCompletion(tx);
        }
    }
}
