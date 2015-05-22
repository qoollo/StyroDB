using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BricksDb.RedisInterface.RedisOperations;
using StyroDB.InMemrory;

namespace BricksDb.RedisInterface.Server
{
    class RedisToStyro:RedisToSmthSystem
    {
        private MemoryDatabase _database;
        private IMemoryTable<string, string> _table;

        protected override void InnerBuild(RedisMessageProcessor processor)
        {
            _database = new MemoryDatabase();
            _table = _database.CreateTable<string, string>("RedisTable");
            processor.AddOperation("SET", new RedisSet(new StyroDataAdapter(_table), "SET"));
        }
    }
}
