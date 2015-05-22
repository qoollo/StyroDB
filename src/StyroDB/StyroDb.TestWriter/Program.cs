using System;
using Qoollo.Concierge;
using Qoollo.Concierge.Extensions;

namespace StyroDb.TestWriter
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            Console.WriteLine("1 - DbWriter\n2 - Distributor\n");
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
            }

            new AppBuilder(true, builder.GetType())
                .EnableControlCommands()
                .EnableInfoCommands()
                .WithDefaultStartupString(DefaultStatupArguments.Debug)
                .Run(args);            

        }       
    }
}
