using System;
using Qoollo.Benchmark.Executor;
using Qoollo.Concierge;
using Qoollo.Concierge.Extensions;
using StyroDB.Adapter.StyroClient;

namespace StyroDb.TestWriter
{
    internal class Program
    {
        const string TableName = "BenchmarkTable";
        private static void Main(string[] args)
        {            
            Console.WriteLine("1 - DbWriter\n2 - Distributor\n3 - Benchmark");
            int choose = int.Parse(Console.ReadLine());

            IUserExecutable builder = null;

            switch (choose)
            {
                case 1:
                    builder = new DbWriterBuilder();
                    break;
                case 2:
                    builder = new DistributorBuilder();
                    break;
                case 3:
                    builder = new BenchmarkExecutor();                    
                    ((BenchmarkExecutor) builder).AddDbFactory(TableName,
                        new StyroDbFactory<long, string>(TableName, new LongStringProvider()));
                    break;
            }

            new AppBuilder(true, builder)
                .EnableControlCommands()
                .EnableInfoCommands()
                .WithDefaultStartupString(DefaultStatupArguments.Debug)
                .Run(args);            

        }       
    }
}
