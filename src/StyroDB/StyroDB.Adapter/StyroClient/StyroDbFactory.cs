using System;
using Qoollo.Client.Support;
using Qoollo.Client.WriterGate;
using Qoollo.Impl.Collector.Parser;
using Qoollo.Impl.Writer.Db;
using StyroDB.Adapter.Internal;

namespace StyroDB.Adapter.StyroClient
{
    public class StyroDbFactory<TKey, TValue> : DbFactory
    {
        private readonly IDataProvider<TKey, TValue> _dataProvider;
        private readonly StyroUserCommandCreator<TKey, TValue> _userCommandCreator;
        private readonly StyroDbModule _dbModule;

        public StyroDbFactory(string tableName, IDataProvider<TKey, TValue> dataProvider)
        {
            _dbModule = new StyroDbModule(null, 10, 10000);
            _dataProvider = dataProvider;
            _userCommandCreator = new StyroUserCommandCreator<TKey, TValue>(tableName);
        }

        public override DbModule Build()
        {
            return new DbLogicModule<StyroCommand, TKey, TValue, StyroConnection, StyroDataReader>(
                new HashFakeImpl<TKey, TValue>(_dataProvider), false, _userCommandCreator, 
                new StyroMetaDataCommandCreator<TKey, TValue>(), _dbModule);
        }

        public override ScriptParser GetParser()
        {
            return null;
        }
    }
}
