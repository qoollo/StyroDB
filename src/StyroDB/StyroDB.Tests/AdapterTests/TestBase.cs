using System;
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
    public class TestBase
    {
        public WriterApi Writer
        {
            get { return _writer; }
        }

        public ICommonNetReceiverWriterForWrite Channel
        {
            get { return _channel; }
        }

        internal IMemoryTable<int, ValueWrapper<int, int>> Table
        {
            get { return _table; }
        }

        internal HashFakeImpl<int, int> Provider
        {
            get { return _provider; }
        }

        private WriterApi _writer;
        private ICommonNetReceiverWriterForWrite _channel;
        private IMemoryTable<int, ValueWrapper<int, int>> _table;
        private HashFakeImpl<int, int> _provider;
        protected const string Host = "localhost";
        protected const int PortForDistributor = 12331;
        protected const int PortForCollector = 12332;
        protected const string TableName = "TestTable";

        [TestInitialize]
        public void Initialize()
        {
            Init();

            var factory = new StyroDbFactory<int, int>(TableName, new IntDataProvider());
            _writer = BuildWriter(PortForDistributor, PortForCollector, factory);

            _channel = OpenChannel<ICommonNetReceiverWriterForWrite>(PortForDistributor);

            _provider = new HashFakeImpl<int, int>(new IntDataProvider());

            _table = GetTable<int, int>(factory, TableName);
        }

        protected WriterApi BuildWriter(int portForDistributor, int portForCollector,
            StyroDbFactory<int, int> factory = null)
        {
            var writer = new WriterApi(new StorageNetConfiguration(Host, portForDistributor, portForCollector),
                new StorageConfiguration(1, Consts.FileWithHashName, Consts.CountRetryWaitAnswerInRestore,
                    Consts.TimeoutWaitAnswerInRestore, TimeSpan.FromMilliseconds(200)), new CommonConfiguration(1, 100));

            writer.Build();
            if (factory == null)
                factory = new StyroDbFactory<int, int>(TableName, new IntDataProvider());
            writer.AddDbModule(factory);
            writer.Start();

            writer.Api.InitDb().IsError.ShouldBeFalse();
            return writer;
        }

        protected virtual void Init()
        {
            var writer = new HashWriter(new HashMapConfiguration(Consts.FileWithHashName,
                    HashMapCreationMode.CreateNew, 1, 1, HashFileType.Writer));
            writer.CreateMap();
            writer.SetServer(0, Host, PortForDistributor, PortForCollector);
            writer.Save();
        }

        private TField GetFiels<TField>(object obj)
        {
            var props = obj.GetType().GetFields(BindingFlags.GetProperty | BindingFlags.Instance
                                        | BindingFlags.NonPublic);
            foreach (var fieldInfo in props)
            {
                if (fieldInfo.FieldType == typeof(TField))
                {
                    return (TField)fieldInfo.GetValue(obj);
                }

            }
            return default(TField);
        }

        internal IMemoryTable<TKey, ValueWrapper<TKey, TValue>> GetTable<TKey, TValue>(
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

        protected T OpenChannel<T>(int port)
        {
            string endPointAddr = string.Format("net.tcp://{0}:{1}/{2}", Host, port, Consts.WcfServiceName);
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

        protected InnerData CreateData(int key, int value, OperationName operationName)
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
        
        [TestCleanup]
        public void Clear()
        {
            Cleanup();
            _writer.Dispose();
        }

        protected virtual void Cleanup()
        {
        }
    }
}
