using System;
using System.Threading;
using Qoollo.Impl.Common;
using Qoollo.Impl.Common.NetResults;
using Qoollo.Impl.Modules.Db.Impl;
using Qoollo.Turbo.ObjectPools;
using StyroDB.Adapter.StyroClient;
using StyroDB.Adapter.Wrappers;
using StyroDB.InMemrory;
using StyroDB.InMemrory.Exceptions;

namespace StyroDB.Adapter.Internal
{
    internal class StyroDbModule :
        DbImplModuleWithPool<StyroConnection, StyroConnectionParams, StyroCommand, StyroDataReader>
    {
        private StyroConnection _connection;

        public StyroDbModule(StyroConnectionParams connectionParam, int maxCountElementInPool, int trimPeriod)
            : base(connectionParam, maxCountElementInPool, trimPeriod)
        {
        }

        public override RemoteResult ExecuteNonQuery(StyroCommand command)
        {
            RemoteResult ret = null;
            using (var element = RentConnection())
            {
                command.Connection = element.Element;
                try
                {
                    command.ExecuteNonQuery();
                    ret = new SuccessResult();
                }
                catch (ObjectDisposedException e)
                {
                    ret = new FailNetResult(e.Message);
                }
                catch (InMemoryException e)
                {
                    ret = new FailNetResult(e.Message);
                }
                
            }

            return ret;
        }

        public override void Start()
        {
            _connection = new StyroConnection(new MemoryDatabaseWrapper(new MemoryDatabase()));
            base.Start();
        }

        public override DbReader<StyroDataReader> CreateReader(StyroCommand command)
        {
            return new StyroReader(command, RentConnection());
        }

        public override RentedElementMonitor<StyroConnection> RentConnectionInner()
        {
            return RentConnection();
        }

        protected override bool CreateElement(out StyroConnection elem, StyroConnectionParams connectionParam,
            int timeout, CancellationToken token)
        {
            elem = _connection;
            return true;
        }

        protected override bool IsValidElement(StyroConnection elem)
        {
            return elem != null;
        }

        protected override void DestroyElement(StyroConnection elem)
        {
            try
            {
                _connection.Dispose();
            }
            catch (Exception)
            {
            }
        }        
    }
}
