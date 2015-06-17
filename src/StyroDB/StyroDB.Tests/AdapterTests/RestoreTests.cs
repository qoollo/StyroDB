using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qoollo.Client.Configuration;
using Qoollo.Client.DistributorGate;
using Qoollo.Client.Support;
using Qoollo.Client.WriterGate;
using Qoollo.Impl.Common.Data.Support;
using Qoollo.Impl.Common.HashFile;
using Qoollo.Impl.Configurations;
using Qoollo.Impl.NetInterfaces.Writer;
using StyroDB.Adapter.Converter;
using StyroDB.Adapter.StyroClient;
using StyroDB.Adapter.Wrappers;
using StyroDB.InMemrory;
using StyroDB.Tests.Helper;

namespace StyroDB.Tests.AdapterTests
{
    /// <summary>
    /// Summary description for RestoreTests
    /// </summary>
    [TestClass]
    public class RestoreTests : TestBase
    {
        private DistributorApi _distributor;
        protected const int PortForDistributor2 = 12341;
        protected const int PortForCollector2 = 12342;
        const int DistributorPortForWriter = 12352;

        private void CreateHashFile(int mode)
        {
            switch (mode)
            {
                case 1:
                {
                    var writer = new HashWriter(new HashMapConfiguration(Consts.FileWithHashName,
                        HashMapCreationMode.CreateNew, 2, 1, HashFileType.Writer));
                    writer.CreateMap();
                    writer.SetServer(0, Host, PortForDistributor, PortForCollector);
                    writer.SetServer(1, Host, PortForDistributor2, PortForCollector2);
                    writer.Save();
                }
                    break;
                case 2:
                    {
                        var writer = new HashWriter(new HashMapConfiguration(Consts.FileWithHashName,
                            HashMapCreationMode.CreateNew, 1, 1, HashFileType.Writer));
                        writer.CreateMap();
                        writer.SetServer(0, Host, PortForDistributor2, PortForCollector2);
                        writer.Save();
                    }
                    break;
            }
        }

        protected override void Init()
        {
            CreateHashFile(1);
            const int distributorPortForProxy = 12351;
            
            _distributor = new DistributorApi(
                new DistributorNetConfiguration(Host, distributorPortForProxy, DistributorPortForWriter),
                new DistributorConfiguration(1), new CommonConfiguration());

            _distributor.Build();
            _distributor.Start();
        }

        private int GetCount(IMemoryTable<int, ValueWrapper<int, int>> table, int countElems,
            Func<ValueWrapper<int, int>, bool> filter)
        {
            int count = 0;
            for (int i = 0; i < countElems; i++)
            {
                ValueWrapper<int, int> value;
                if (table.Read(i, out value) && filter(value))
                    count++;
            }
            return count;
        }

        [TestMethod]
        public void Writer_RestoreTwoServers_DeleteOneFirstServerEqualLocalOnSecondServer()
        {
            const int count = 10;            
            for (int i = 0; i < count; i++)
            {                
                Channel.ProcessSync(CreateData(i, i, OperationName.Create));
                var data = Table.Read(i);
                data.Value.ShouldBeEqualTo(i);                
            }

            int local = GetCount(Table, count, data => data.StyroMetaData.IsLocal);

            var factory = new StyroDbFactory<int, int>(TableName, new IntDataProvider());
            var writer = BuildWriter(PortForDistributor2, PortForCollector2, factory);
            var table = GetTable<int, int>(factory, TableName);

            writer.Api.Restore(new ServerAddress(Host, DistributorPortForWriter), false);
            Thread.Sleep(1000);
            
            int delete = GetCount(Table, count, data => data.StyroMetaData.IsDelete);
            delete.ShouldBeEqualTo(count - local);

            GetCount(table, count, data => data.StyroMetaData.IsLocal).ShouldBeEqualTo(delete);

            writer.Dispose();
        }

        [TestMethod]
        public void Writer_RestoreTwoServers_UpdateModel_DeleteOneFirstServerEqualLocalOnSecondServer()
        {
            CreateHashFile(2);   
            
            var factory = new StyroDbFactory<int, int>(TableName, new IntDataProvider());
            var writer = BuildWriter(PortForDistributor2, PortForCollector2, factory);
            var table = GetTable<int, int>(factory, TableName);

            const int count = 10;

            var channel = OpenChannel<ICommonNetReceiverWriterForWrite>(PortForDistributor2);
            for (int i = 0; i < count; i++)
            {
                channel.ProcessSync(CreateData(i, i, OperationName.Create));
                var data = table.Read(i);
                data.Value.ShouldBeEqualTo(i);
            }
            GetCount(table, count, data => data.StyroMetaData.IsLocal).ShouldBeEqualTo(count);

            CreateHashFile(1);
            writer.Api.UpdateModel();
            Writer.Api.Restore(new ServerAddress(Host, DistributorPortForWriter), true);
            Thread.Sleep(1000);

            var total = GetCount(Table, count, data => data.StyroMetaData.IsLocal) +
                        GetCount(table, count, data => !data.StyroMetaData.IsDelete);
            total.ShouldBeEqualTo(count);
            writer.Dispose();
        }

        protected override void Cleanup()
        {
            _distributor.Dispose();
        }
    }
}
