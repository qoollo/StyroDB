using System;
using System.Collections.Generic;
using System.Data;
using Qoollo.Impl.Common;
using Qoollo.Impl.Modules.Db.Impl;
using Qoollo.Impl.Writer.Db.Commands;
using StyroDB.Adapter.StyroClient.Commands;
using StyroDB.Adapter.Wrappers;

namespace StyroDB.Adapter.StyroClient
{
    internal class StyroUserCommandCreator<TKey, TValue> :
        IUserCommandCreator<StyroCommand, StyroConnection, TKey, TValue, StyroDataReader>
    {
        private readonly string _tableName;

        public StyroUserCommandCreator(string tableName)
        {
            _tableName = tableName;
        }

        public bool CreateDb(StyroConnection connection)
        {
            connection.CreateTable<TKey, TValue>(_tableName);
            return true;
        }

        public string GetKeyName()
        {
            return string.Empty;
        }

        public List<string> GetTableNameList()
        {
            return new List<string> {_tableName};
        }

        public string GetKeyInitialization()
        {
            return string.Empty;
        }

        public void GetFieldsDescription(DataTable dataTable)
        {
            
        }

        public StyroCommand Create(TKey key, TValue value)
        {
            return new StyroCommandBuilder<TKey, TValue>(_tableName).ExecuteNonQuery(
                    table => table.Write(key, value));
        }

        public StyroCommand Update(TKey key, TValue value)
        {
            return Create(key, value);
        }

        public StyroCommand Delete(TKey key)
        {
            return new StyroCommandBuilder<TKey, TValue>(_tableName).ExecuteNonQuery(
                    table => table.Delete(key));
        }

        public StyroCommand Read()
        {
            return new StyroCommandBuilder<TKey, ValueWrapper<TKey, TValue>>(_tableName)
                .ReadCommand();
        }

        public TValue ReadObjectFromReader(DbReader<StyroDataReader> reader, out TKey key)
        {
            var obj = reader.Reader.Values;
            var value = (ValueWrapper<TKey, TValue>)reader.Reader.Values[0];
            key = value.StyroMetaData.Key;
            return value.Value;
        }

        public TValue ReadObjectFromSearchData(List<Tuple<object, string>> fields)
        {
            throw new NotImplementedException();
        }

        public RemoteResult CustomOperation(StyroConnection connection, TKey key, byte[] value, string description)
        {
            throw new NotImplementedException();
        }

        public RemoteResult CustomOperationRollback(StyroConnection connection, TKey key, byte[] value,
            string description)
        {
            throw new NotImplementedException();
        }
    }
}
