using System;
using BricksDb.RedisInterface.Server;

namespace BricksDb.RedisInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1 - To Briks\n2 - To Writer\n");
            int choose = int.Parse(Console.ReadLine());

            RedisToSmthSystem builder = null;

            switch (choose)
            {
                case 1:
                    builder = new RedisToBriks();
                    break;
                case 2:
                    builder = new RedisToDbWriter();
                    break;
            }


            builder.Build();
            builder.Start();
            Console.WriteLine("Press enter to stop");
            Console.ReadLine();
            builder.Stop();
        }
    }
}
