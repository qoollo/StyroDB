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
using StyroDB.Adapter.Converter;
using StyroDB.Adapter.StyroClient;
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

        protected override void Init()
        {
            var writer = new HashWriter(new HashMapConfiguration(Consts.FileWithHashName,
                    HashMapCreationMode.CreateNew, 2, 1, HashFileType.Writer));
            writer.CreateMap();
            writer.SetServer(0, Host, PortForDistributor, PortForCollector);
            writer.SetServer(1, Host, PortForDistributor2, PortForCollector2);
            writer.Save();

            const int distributorPortForProxy = 12351;
            
            _distributor = new DistributorApi(
                new DistributorNetConfiguration(Host, distributorPortForProxy, DistributorPortForWriter),
                new DistributorConfiguration(1), new CommonConfiguration());

            _distributor.Build();
            _distributor.Start();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var db = new MemoryDatabase();
            
            const int count = 10;
            int local = 0;
            for (int i = 0; i < count; i++)
            {
                Channel.ProcessSync(CreateData(i, i, OperationName.Create));
                var data = Table.Read(i);
                data.Value.ShouldBeEqualTo(i);
                if (data.StyroMetaData.IsLocal)
                    local++;
            }
           
            var factory = new StyroDbFactory<int, int>(TableName, new IntDataProvider());
            var writer = BuildWriter(PortForDistributor2, PortForCollector2, factory);
            var table = GetTable<int, int>(factory, TableName);

            Writer.Api.Restore(new ServerAddress(Host, DistributorPortForWriter), false);
            Thread.Sleep(1000000);

            writer.Dispose();
        }

        protected override void Cleanup()
        {
            _distributor.Dispose();
        }


        private class TestClass
        {
           public int Value { get; set; }
        }
    }
}
