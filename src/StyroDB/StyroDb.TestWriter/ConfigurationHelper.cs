using System;
using System.Collections.Specialized;
using System.Configuration;

namespace StyroDb.TestWriter
{
    class ConfigurationHelper
    {
        private static volatile ConfigurationHelper _instance;
        private static readonly object SyncObj = new Object();
        public string DbWriterHost;
        public int PortForDistributor;
        public int PortForCollector;
        public int CountThreadsWriter;
        public int CountReplicsWriter;

        public string DistributorHost;
        public int CountReplicsDistibutor;
        public int CountThreadsDistributor;
        public int PortForWriter;
        public int PortForProxy;

        public static ConfigurationHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncObj)
                    {
                        if (_instance == null)
                            _instance = new ConfigurationHelper();
                    }
                }

                return _instance;
            }
        }

        public ConfigurationHelper()
        {
            DbWriterConfiguration();
            DistributorConfiguration();
        }

        private void DbWriterConfiguration()
        {
            var section = ConfigurationManager.GetSection("DbWriter") as NameValueCollection;
            DbWriterHost = section["host"];
            PortForDistributor = int.Parse(section["portForDistributor"]);
            PortForCollector = int.Parse(section["portForCollector"]);
            CountThreadsWriter = int.Parse(section["countTreads"]);
            CountReplicsWriter = int.Parse(section["countReplics"]);
        }

        private void DistributorConfiguration()
        {
            var section = ConfigurationManager.GetSection("Distributor") as NameValueCollection;
            DistributorHost = section["host"];
            PortForProxy = int.Parse(section["portForProxy"]);
            PortForWriter = int.Parse(section["portForDbWriter"]);
            CountThreadsDistributor = int.Parse(section["countTreads"]);
            CountReplicsDistibutor = int.Parse(section["countReplics"]);
        }        
    }
}
