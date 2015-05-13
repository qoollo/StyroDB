using BricksDb.RedisInterface.Server;
using Qoollo.Client.Request;
using Qoollo.Client.Support;
using Qoollo.Impl.Common.Data.DataTypes;
using Qoollo.Impl.Common.Data.Support;
using Qoollo.Impl.Common.Data.TransactionTypes;
using Qoollo.Impl.Common.Server;
using Qoollo.Impl.Configurations;
using Qoollo.Impl.NetInterfaces.Data;
using Qoollo.Impl.NetInterfaces.Writer;
using StyroDb.TestWriter;

namespace BricksDb.RedisInterface.RedisOperations
{
    class DbWriterDataAdapter:IDataAdapter
    {
        private readonly string _tableName;
        private readonly StringDataProvider _stringDataProvider;
        private readonly ICommonNetReceiverWriterForWrite _channel;        

        public DbWriterDataAdapter(string tableName, StringDataProvider stringDataProvider)
        {
            _tableName = tableName;
            _stringDataProvider = stringDataProvider;
            var factory =
                NetConnector.Connect<ICommonNetReceiverWriterForWrite>(
                    new ServerId(ConfigurationHelper.Instance.DbWriterHost,
                        ConfigurationHelper.Instance.PortForDistributor), Consts.WcfServiceName,
                    new ConnectionTimeoutConfiguration(Consts.OpenTimeout, Consts.SendTimeout));
            _channel = factory.CreateChannel();            
        }

        public string Read(string key, out RequestDescription result)
        {            
            _channel.ReadOperation(new InnerData(new Transaction("", "")
            {
                OperationName = OperationName.Read,
                OperationType = OperationType.Sync,
                TableName = _tableName
            })
            {
                Key = _stringDataProvider.SerializeKey(key)
            });

            result = new RequestDescription();
            return "";
        }

        public RequestDescription Create(string key, string value)
        {
            var result = _channel.ProcessSync(new InnerData(new Transaction("", "")
            {
                OperationName = OperationName.Create,
                OperationType = OperationType.Sync,
                TableName = _tableName
            })
            {
                Key = _stringDataProvider.SerializeKey(key),
                Data = _stringDataProvider.SerializeValue(value)
            });

            return new RequestDescription(result);
        }
    }
}
