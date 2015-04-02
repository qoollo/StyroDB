using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using StyroDB.Adapter.Wrappers;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.StyroClient.Commands
{
    internal class StyroCommandBuilder<TKey, TValue>
    {
        private readonly string _tableName;
        private bool _wrapperReader = false;
        public StyroConnection Connection { get; private set; }

        public StyroCommandBuilder(string tableName)
        {
            Contract.Requires(!string.IsNullOrEmpty(tableName));
            _tableName = tableName;
        }

        public StyroCommandBuilder<TKey, TValue> SetWrapperReader()
        {
            _wrapperReader = true;
            return this;
        }

        public StyroCommand ExecuteNonQuery(Action<TKey, IMemoryTable<TKey, TValue>> actionWithKey)
        {
            Contract.Requires(actionWithKey != null);
            var ret = new StyroCommandGeneric<TKey>(_tableName);
            ret.ExecuteNonQueryInner =
                (connection, s) => actionWithKey(ret.Key, connection.GetTable<TKey, TValue>(s));
            return ret;
        }

        public StyroCommand ExecuteNonQuery(Action<IMemoryTable<TKey, TValue>> actionWihtoutKey)
        {
            Contract.Requires(actionWihtoutKey != null);
            return new StyroCommand(_tableName)
            {
                ExecuteNonQueryInner =
                    (connection, s) => actionWihtoutKey(connection.GetTable<TKey, TValue>(s))
            };
        }

        public StyroCommand ExecuteNonQuery(Action<StyroConnection> actionDatabase)
        {
            Contract.Requires(actionDatabase != null);
            return new StyroCommand(_tableName)
            {
                ExecuteNonQueryInner = (connection, s) => actionDatabase(connection)
            };
        }

        public StyroCommand ExecuteReader(Func<IEnumerable<TValue>, IEnumerable<TValue>> filter)
        {
            Contract.Requires(filter != null);
            var command =  new StyroCommand(_tableName)
            {
                ExecuteReaderInner = (connection, s) =>
                {
                    Connection = connection;
                    var table = connection.GetTable<TKey, TValue>(s);
                    var result = table.Query(filter);

                    return result.Cast<object>();
                }
            };

            if (_wrapperReader)
                command.ExecuteReaderFunc = objects => new StyroDataReaderWithWrapper<TKey, TValue>(objects);
            return command;
        }

        public StyroCommand ReadCommand(TKey key)
        {
            return new StyroCommand(_tableName)
            {
                ExecuteReaderInner = (connection, s) =>
                {
                    var list = new List<object>();
                    var table = connection.GetTable<TKey, TValue>(_tableName);
                    try
                    {
                        var value = table.Read(key);
                        list.Add(value);
                    }
                    catch (KeyNotFoundException)
                    {
                    }

                    return list;
                }
            };
        }

        public StyroCommand ReadCommand()
        {
            var command = new StyroCommandGeneric<TKey>(_tableName);
            command.ExecuteReaderInner = (connection, s) =>
            {
                var list = new List<object>();
                var table = connection.GetTable<TKey, TValue>(_tableName);
                try
                {
                    var value = table.Read(command.Key);
                    list.Add(value);
                }
                catch (KeyNotFoundException)
                {
                }

                return list;
            };

            if (_wrapperReader)
                command.ExecuteReaderFunc = objects => new StyroDataReaderWithWrapper<TKey, TValue>(objects);
            return command;
        }

        public StyroCommand ReadAllDataCommand()
        {
            var ret = new StyroCommandGeneric<TKey>(_tableName);
            ret.ExecuteReaderInner = (connection, s) =>
            {
                var table = connection.GetTable<TKey, ValueWrapper<TKey, TValue>>(_tableName);
                return table.Query(x => x).Cast<object>().ToList();
            };
            return ret;
        }
    }
}
