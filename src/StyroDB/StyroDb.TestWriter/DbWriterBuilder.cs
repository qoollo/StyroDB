using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qoollo.Client.Configuration;
using Qoollo.Client.Support;
using Qoollo.Client.WriterGate;
using Qoollo.Concierge;
using Qoollo.Concierge.Attributes;
using Qoollo.Concierge.Commands;
using StyroDB.Adapter.StyroClient;

namespace StyroDb.TestWriter
{
    public class RestoreCommand : UserCommand
    {
        [Parameter(ShortKey = 'f',Description = "Update hash file", DefaultValue = false, IsRequired = false)]
        public bool IsModelUpdated { get; set; }

        [Parameter(ShortKey = 'h', Description = "Distributor host", IsRequired = true)]
        public string DistributorHost { get; set; }

        [Parameter(ShortKey = 'p', Description = "Distributor port", IsRequired = true)]
        public int DistributorPort { get; set; }
    }

    internal class DbWriterBuilder : IUserExecutable
    {
        private WriterApi _writer;

        [CommandHandler("restore", "Start restore")]
        public string RestoreCommand(RestoreCommand command)
        {
            return _writer.Api.Restore(new ServerAddress(command.DistributorHost, command.DistributorPort),
                command.IsModelUpdated).ToString();
        }

        [CommandHandler("update", "Update hash file")]
        public string UpdateModelCommand(UserCommand command)
        {
            _writer.Api.UpdateModel();
            return string.Empty;
        }

        public void Start()
        {
            //Qoollo.Logger.Initialization.Initializer.InitializeLoggerInAssembly(
            //    LoggerDefault.ConsoleLogger, typeof(WriterApi).Assembly);


            _writer = new WriterApi(new StorageNetConfiguration(ConfigurationHelper.Instance.DbWriterHost,
                ConfigurationHelper.Instance.PortForDistributor, ConfigurationHelper.Instance.PortForCollector),
                new StorageConfiguration(1, Consts.FileWithHashName, Consts.CountRetryWaitAnswerInRestore,
                    Consts.TimeoutWaitAnswerInRestore, TimeSpan.FromMinutes(1),
                    Consts.PeriodDeleteAfterRestore, periodStartDelete: TimeSpan.FromDays(1)),
                new CommonConfiguration(ConfigurationHelper.Instance.CountThreadsWriter, 100));
            _writer.Build();
            _writer.AddDbModule(new StyroDbFactory<int, int>("TestTable", new IntDataProvider()));
            _writer.AddDbModule(new StyroDbFactory<string, string>("RedisTable", new StringDataProvider()));
            _writer.AddDbModule(new StyroDbFactory<long, string>("BenchmarkTable", new LongStringProvider()));
            
            _writer.Start();

            _writer.Api.InitDb();
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
