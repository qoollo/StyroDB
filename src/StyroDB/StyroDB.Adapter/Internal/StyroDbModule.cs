using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qoollo.Impl.Common;
using Qoollo.Impl.Modules.Db.Impl;
using Qoollo.Turbo.ObjectPools;
using StyroDB.Adapter.StyroClient;

namespace StyroDB.Adapter.Internal
{
    internal class StyroDbModule :
        DbImplModuleWithPool<StyroConnection, StyroConnectionParams, StyroCommand, StyroDataReader>
    {
        public StyroDbModule(StyroConnectionParams connectionParam, int maxCountElementInPool, int trimPeriod)
            : base(connectionParam, maxCountElementInPool, trimPeriod)
        {
        }

        public override RemoteResult ExecuteNonQuery(StyroCommand command)
        {
            //RemoteResult ret = null;
            //using (var element = RentConnection())
            //{
            //    command.Connection = element.Element;
            //    try
            //    {
            //        command.ExecuteNonQuery();
            //        ret = new SuccessResult();
            //    }
            //    catch (SqlException e)
            //    {
            //        Logger.Logger.Instance.Error(e, "");
            //        ret = new FailNetResult(e.Message);
            //    }
            //    catch (IOException e)
            //    {
            //        Logger.Logger.Instance.Error(e, "");
            //        ret = new FailNetResult(e.Message);
            //    }
            //    catch (InvalidOperationException e)
            //    {
            //        Logger.Logger.Instance.Error(e, "");
            //        ret = new FailNetResult(e.Message);
            //    }
            //}

            //return ret;
            throw new NotImplementedException();
        }

        public override DbReader<StyroDataReader> CreateReader(StyroCommand command)
        {
            throw new NotImplementedException();
        }

        public override RentedElementMonitor<StyroConnection> RentConnectionInner()
        {
            throw new NotImplementedException();
        }

        protected override bool CreateElement(out StyroConnection elem, StyroConnectionParams connectionParam,
            int timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected override bool IsValidElement(StyroConnection elem)
        {
            throw new NotImplementedException();
        }

        protected override void DestroyElement(StyroConnection elem)
        {
            throw new NotImplementedException();
        }
    }
}
