using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qoollo.Client.Configuration;
using Qoollo.Client.Support;
using Qoollo.Client.WriterGate;
using Qoollo.Impl.Common.Data.DataTypes;
using Qoollo.Impl.Common.Data.Support;
using Qoollo.Impl.Common.Data.TransactionTypes;
using Qoollo.Impl.Common.HashFile;
using Qoollo.Impl.Configurations;
using Qoollo.Impl.NetInterfaces.Writer;
using StyroDB.Adapter.Internal;
using StyroDB.Adapter.StyroClient;
using StyroDB.Adapter.Wrappers;
using StyroDB.InMemrory;
using StyroDB.Tests.Helper;

namespace StyroDB.Tests.AdapterTests
{    
    [TestClass]
    public class StyroWriterTests
    {
        private WriterApi _writer;
        private ICommonNetReceiverWriterForWrite _channel;
        private IntDataProvider _provider;
        private IMemoryTable<int, ValueWrapper<int, int>> _table;
        private const string Host = "localhost";
        private const int PortForDistributor = 12331;
        private const int PortForCollector = 12332;
        private const string TableName = "TestTable";

        [TestInitialize]
        public void Initialize()
        {
            var writer = new HashWriter(new HashMapConfiguration(Consts.FileWithHashName,
                    HashMapCreationMode.CreateNew, 1, 1, HashFileType.Writer));
            writer.CreateMap();
            writer.SetServer(0, Host, PortForDistributor, PortForCollector);
            writer.Save();

            _writer = new WriterApi(new StorageNetConfiguration(Host, PortForDistributor, PortForCollector),
                new StorageConfiguration(1), new CommonConfiguration(1, 100));

            _provider = new IntDataProvider();
            _writer.Build();
            var factory = new StyroDbFactory<int, int>(TableName, _provider);
            _writer.AddDbModule(factory);
            _writer.Start();

            _writer.Api.InitDb().IsError.ShouldBeFalse();

            _channel = OpenChannel<ICommonNetReceiverWriterForWrite>();

            _table = GetTable<int, int>(factory, TableName);
        }

        public T OpenChannel<T>()
        {
            string endPointAddr = string.Format("net.tcp://{0}:{1}/{2}", Host, PortForDistributor, Consts.WcfServiceName);
            var endpointAddress =
                new EndpointAddress(endPointAddr);

            var tcpBinding = new NetTcpBinding
            {
                Security = { Mode = SecurityMode.None },
                MaxReceivedMessageSize = 2147483647,
                MaxBufferSize = 2147483647,                
            };

            var cf = new ChannelFactory<T>(tcpBinding, endpointAddress);
            foreach (OperationDescription op in cf.Endpoint.Contract.Operations)
            {
                var dataContractBehavior = op.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dataContractBehavior != null)
                {
                    dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
                }
            }
            return cf.CreateChannel();
        }

        [TestCleanup]
        public void Clear()
        {
            _writer.Dispose();
        }

        private InnerData CreateData(int key, int value, OperationName operationName)
        {
            var data = new InnerData(new Transaction(_provider.CalculateHashFromKey(key), "123"))
            {
                Data = _provider.SerializeValue(value),
                Key = _provider.SerializeKey(key),
                Transaction =
                {
                    TableName = TableName,
                    OperationName = operationName
                },
            };
            return data;
        }

        private TField GetFiels<TField>(object obj)
        {
            var props = obj.GetType().GetFields(BindingFlags.GetProperty | BindingFlags.Instance
                                        | BindingFlags.NonPublic);
            foreach (var fieldInfo in props)
            {
                if (fieldInfo.FieldType == typeof (TField))
                {
                    return (TField)fieldInfo.GetValue(obj);
                }

            }
            return default(TField);
        }

        private IMemoryTable<TKey, ValueWrapper<TKey, TValue>> GetTable<TKey, TValue>(
            StyroDbFactory<int, int> factory, string tableName)
        {
            var obj = GetFiels<StyroDbModule>(factory);
            var obj1 = GetFiels<StyroConnection>(obj);
            var tables = obj1.Tables;
            foreach (var table in tables)
            {
                if (table.TableName == tableName)
                {
                    var bTable = table as TableHandler<TKey, ValueWrapper<TKey, TValue>>;
                    if (bTable != null) return bTable.Table;
                }
            }
            return null;
        }

        [TestMethod]
        public void DbModule_ProcessSync_WriteValue()
        {
            var data = CreateData(1, 1, OperationName.Create);
            _channel.ProcessSync(data).ShouldBeNull();
            _table.Read(1).Value.ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_WriteDiplicateValue_NoException()
        {
            var data = CreateData(1, 1, OperationName.Create);
            _channel.ProcessSync(data).ShouldBeNull();
            _channel.ProcessSync(data).ShouldBeNull();

            _table.Read(1).Value.ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_WriteAndReadValue()
        {
            var data = CreateData(1, 1, OperationName.Create);
            _channel.ProcessSync(data).ShouldBeNull();

            data = CreateData(1, 100, OperationName.Read);
            data = _channel.ReadOperation(data);

            _provider.DeserializeValue(data.Data).ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_ReadValueWithoutWrite()
        {
            var data = CreateData(1, 100, OperationName.Read);
            data = _channel.ReadOperation(data);

            data.Data.ShouldBeNull();
        }

        [TestMethod]
        public void DbModule_ProcessSync_WriteAndReadCountElements()
        {
            int count = 100;
            for (int i = 0; i < count; i++)
            {
                var data = CreateData(i, i, OperationName.Create);
                _channel.ProcessSync(data).ShouldBeNull();    
            }

            for (int i = 0; i < count; i++)
            {
                var data = CreateData(i, i+100, OperationName.Read);
                data = _channel.ReadOperation(data);

                _provider.DeserializeValue(data.Data).ShouldBeEqualTo(i);
            }


        }

        [TestMethod]
        public void DbModule_ProcessSync_UpdateWithWrite_OnlyOneValue()
        {
            var data = CreateData(1, 1, OperationName.Create);
            _channel.ProcessSync(data).ShouldBeNull();
            _table.Read(1).Value.ShouldBeEqualTo(1);

            data = CreateData(1, 10, OperationName.Update);
            _channel.ProcessSync(data).ShouldBeNull();

            _table.Read(1).Value.ShouldBeEqualTo(10);

            _table.Query(x => x).Count().ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_UpdateWithoutWrite()
        {
            var data = CreateData(1, 10, OperationName.Update);
            _channel.ProcessSync(data).ShouldBeNull();

            _table.Read(1).Value.ShouldBeEqualTo(10);

            _table.Query(x => x).Count().ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_DeleteWithWrite()
        {
            var data = CreateData(1, 1, OperationName.Create);
            _channel.ProcessSync(data).ShouldBeNull();

            data = CreateData(1, 100, OperationName.Delete);
            _channel.ProcessSync(data).ShouldBeNull();

            data = CreateData(1, 100, OperationName.Read);
            data = _channel.ReadOperation(data);

            data.Data.ShouldBeNull();
        }

        [TestMethod]
        public void DbModule_ProcessSync_DeleteWithoutWrite()
        {
            var data = CreateData(1, 100, OperationName.Delete);
            _channel.ProcessSync(data).ShouldBeNull();

            data = CreateData(1, 100, OperationName.Read);
            data = _channel.ReadOperation(data);

            data.Data.ShouldBeNull();
        }

        [TestMethod]
        public void DbModule_ProcessSync_Select()
        {   

        }
    }
}
