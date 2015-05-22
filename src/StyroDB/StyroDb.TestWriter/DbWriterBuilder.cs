using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qoollo.Client.Configuration;
using Qoollo.Client.Support;
using Qoollo.Client.WriterGate;
using Qoollo.Concierge;
using Qoollo.Logger;
using StyroDB.Adapter.StyroClient;

namespace StyroDb.TestWriter
{
    internal class DbWriterBuilder : IUserExecutable
    {
        private WriterApi _writer;

        public void Start()
        {
            Qoollo.Logger.Initialization.Initializer.InitializeLoggerInAssembly(
                LoggerDefault.ConsoleLogger, typeof (WriterApi).Assembly);


            _writer = new WriterApi(new StorageNetConfiguration(ConfigurationHelper.Instance.DbWriterHost,
                ConfigurationHelper.Instance.PortForDistributor, ConfigurationHelper.Instance.PortForCollector),
                new StorageConfiguration(1, Consts.FileWithHashName, Consts.CountRetryWaitAnswerInRestore,
                    Consts.TimeoutWaitAnswerInRestore, Consts.TimeoutSendAnswerInRestore,
                    Consts.PeriodDeleteAfterRestore, periodStartDelete: TimeSpan.FromDays(1)),
                new CommonConfiguration(ConfigurationHelper.Instance.CountThreadsWriter, 100));
            _writer.Build();
            _writer.AddDbModule(new StyroDbFactory<int, int>("TestTable", new IntDataProvider()));
            _writer.AddDbModule(new StyroDbFactory<string, string>("RedisTable", new StringDataProvider()));
            _writer.AddDbModule(new StyroDbFactory<long, string>("BenchmarkTable", new LongStringProvider()));
            
            _writer.Start();
        }

        public void Stop()
        {
            _writer.Dispose();
        }

        public void Dispose()
        {
        }

        public IWindowsServiceConfig Configuration
        {
            get
            {
                return new WinServiceConfig
                {
                    Async = true,
                    DisplayName = "StyroDbWriter",
                    InstallName = "StyroDbWriter",
                    StartAfterInstall = true
                };
            }
        }
    }
}
