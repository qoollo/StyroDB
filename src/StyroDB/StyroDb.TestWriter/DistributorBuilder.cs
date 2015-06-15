using System;
using Qoollo.Client.Configuration;
using Qoollo.Client.DistributorGate;
using Qoollo.Client.Support;
using Qoollo.Concierge;
using Qoollo.Logger;

namespace StyroDb.TestWriter
{
    class DistributorBuilder : IUserExecutable
    {
        private DistributorApi _distributor;

        public void Start()
        {            
            //Qoollo.Logger.Initialization.Initializer.InitializeLoggerInAssembly(
            //    LoggerDefault.ConsoleLogger, typeof (DistributorApi).Assembly);

            var net = new DistributorNetConfiguration(ConfigurationHelper.Instance.DistributorHost,
                ConfigurationHelper.Instance.PortForProxy, ConfigurationHelper.Instance.PortForWriter,
                Consts.WcfServiceName, 10);

            var distributorConfiguration =
                new DistributorConfiguration(ConfigurationHelper.Instance.CountReplicsDistibutor,
                    Consts.FileWithHashName, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(60));

            var commonConfiguration = new CommonConfiguration(ConfigurationHelper.Instance.CountThreadsDistributor, 10000);

            _distributor = new DistributorApi(net, distributorConfiguration, commonConfiguration,
                   new TimeoutConfiguration(TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(10000)));

            _distributor.Build();
            _distributor.Start();
        }

        public void Stop()
        {
            _distributor.Dispose();
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
                    DisplayName = "StyroDistributor",
                    InstallName = "StyroDistributor",
                    StartAfterInstall = true
                };
            }
        }
    }
}
