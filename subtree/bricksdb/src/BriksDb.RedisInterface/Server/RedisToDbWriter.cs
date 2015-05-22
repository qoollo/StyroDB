using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BricksDb.RedisInterface.RedisOperations;
using Qoollo.Client.Configuration;
using Qoollo.Client.Support;
using Qoollo.Client.WriterGate;
using StyroDB.Adapter.StyroClient;
using StyroDb.TestWriter;

namespace BricksDb.RedisInterface.Server
{
    class RedisToDbWriter:RedisToSmthSystem
    {
        private readonly WriterApi _writer;

        public RedisToDbWriter()
        {
            _writer = new WriterApi(new StorageNetConfiguration(ConfigurationHelper.Instance.DbWriterHost,
                ConfigurationHelper.Instance.PortForDistributor, ConfigurationHelper.Instance.PortForCollector),
                new StorageConfiguration(1, Consts.FileWithHashName, Consts.CountRetryWaitAnswerInRestore,
                    Consts.TimeoutWaitAnswerInRestore, Consts.TimeoutSendAnswerInRestore,
                    Consts.PeriodDeleteAfterRestore, TimeSpan.FromDays(1)),
                new CommonConfiguration(ConfigurationHelper.Instance.CountThreadsWriter, 10000));
        }

        protected override void InnerBuild(RedisMessageProcessor processor)
        {
            var provider = new StringDataProvider();
            _writer.Build();
            _writer.AddDbModule(new StyroDbFactory<int, int>("TestTable", new IntDataProvider()));
            _writer.AddDbModule(new StyroDbFactory<string, string>("RedisTable", new StringDataProvider()));

            processor.AddOperation("SET", new RedisSet(new DbWriterDataAdapter("RedisTable", provider), "SET"));
            processor.AddOperation("GET", new RedisGet(new DbWriterDataAdapter("RedisTable", provider), "GET"));
        }

        public override void Start()
        {
            _writer.Start();
            _writer.Api.InitDb();
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            _writer.Dispose();
        }
    }
}
