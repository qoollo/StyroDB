using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qoollo.Client.Configuration;
using Qoollo.Client.Support;
using Qoollo.Client.WriterGate;
using Qoollo.Impl.Common.HashFile;
using Qoollo.Impl.Configurations;
using StyroDB.Adapter.StyroClient;

namespace StyroDb.TestWriter
{
    internal class Program
    {
        private static WriterApi _writer;

        private static void Main(string[] args)
        {
            const string host = "localhost";
            const int portForDistributor = 12331;
            const int portForCollector = 12332;
            const string tableName = "TestTable";

            var writer = new HashWriter(new HashMapConfiguration(Consts.FileWithHashName,
                HashMapCreationMode.CreateNew, 1, 1, HashFileType.Writer));
            writer.CreateMap();
            writer.SetServer(0, host, portForDistributor, portForCollector);
            writer.Save();

            _writer = new WriterApi(new StorageNetConfiguration(host, portForDistributor, portForCollector),
                new StorageConfiguration(1, Consts.FileWithHashName, Consts.CountRetryWaitAnswerInRestore,
                    Consts.TimeoutWaitAnswerInRestore, Consts.TimeoutSendAnswerInRestore,
                    Consts.PeriodDeleteAfterRestore, periodStartDelete:TimeSpan.FromDays(1)),
                new CommonConfiguration(1, 100));

            try
            {
                _writer.Build();
                _writer.AddDbModule(new StyroDbFactory<int, int>(tableName, new IntDataProvider()));
                _writer.Start();

                _writer.Api.InitDb();
                Console.WriteLine("Press enter for exit");
                Console.ReadLine();
            

            }
            finally
            {
                _writer.Dispose();
            }

            
        }

        private class IntDataProvider : CommonDataProvider<int, int>
        {
            public override string CalculateHashFromKey(int key)
            {
                return key.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }
        }

    }
}
