using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qoollo.Impl.Collector.Parser;
using Qoollo.Impl.Common.Data.DataTypes;
using Qoollo.Impl.Modules.Db.Impl;
using Qoollo.Impl.Writer.Db.Commands;
using StyroDB.Adapter.StyroClient;
using StyroDB.Adapter.StyroClient.Commands;
using StyroDB.Adapter.Wrappers;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.Internal
{
    internal class StyroMetaDataCommandCreator<TKey, TValue> : IMetaDataCommandCreator<StyroCommand, StyroReader>
    {
        private string _tableName;

        #region Not Implemented

        public void SetKeyName(string keyName)
        {
        }

        #endregion

        public void SetTableName(List<string> tableName)
        {
            _tableName = tableName.First();
        }

        public StyroCommand InitMetaDataDb(string idInit)
        {
            return new StyroCommandNonQuery<TKey, ValueWrapper<TKey, TValue>>(_tableName, tableName => { });
        }

        public StyroCommand CreateMetaData(bool remote)
        {
            return new StyroCommandNonQueryWithId<TKey, ValueWrapper<TKey, TValue>>(_tableName,
                (key, table) =>
                {
                    var value = table.Read(key);
                    value.MetaData.IsLocal = remote;
                    table.Write(key, value);
                });
        }

        public StyroCommand DeleteMetaData()
        {
            return new StyroCommandNonQueryWithId<TKey, ValueWrapper<TKey, TValue>>(_tableName,
                (key, table) => table.Delete(key));
        }

        public StyroCommand UpdateMetaData(bool local)
        {
            return new StyroCommandNonQueryWithId<TKey, ValueWrapper<TKey, TValue>>(_tableName,
                (key, table) =>
                {
                    var value = table.Read(key);
                    value.MetaData.IsLocal = local;
                    table.Write(key, value);
                });
        }

        public StyroCommand SetDataDeleted()
        {
            return new StyroCommandNonQueryWithId<TKey, ValueWrapper<TKey, TValue>>(_tableName,
                (key, table) =>
                {
                    var value = table.Read(key);
                    value.MetaData.IsDelete = true;
                    value.MetaData.DeleteTime = DateTime.Now;
                    table.Write(key, value);
                });
        }

        public StyroCommand SetDataNotDeleted()
        {
            return new StyroCommandNonQueryWithId<TKey, ValueWrapper<TKey, TValue>>(_tableName,
                (key, table) =>
                {
                    var value = table.Read(key);
                    value.MetaData.IsDelete = false;
                    table.Write(key, value);
                });
        }

        public StyroCommand ReadMetaData(StyroCommand userRead)
        {
            return new StyroCommandReadWithId<TKey, ValueWrapper<TKey, TValue>>(_tableName);
        }

        public Tuple<MetaData, bool> ReadMetaDataFromReader(DbReader<StyroReader> reader, bool readuserId = true)
        {
            throw new NotImplementedException();
        }

        public MetaData ReadMetaFromSearchData(SearchData data)
        {
            throw new NotImplementedException();
        }

        public string ReadWithDeleteAndLocal(bool isDelete, bool local)
        {
            throw new NotImplementedException();
        }

        public StyroCommand ReadWithDelete(StyroCommand userRead, bool isDelete)
        {
            throw new NotImplementedException();
        }

        public StyroCommand ReadWithDeleteAndLocal(StyroCommand userRead, bool isDelete, bool local)
        {
            throw new NotImplementedException();
        }

        public StyroCommand CreateSelectCommand(string script, FieldDescription idDescription, List<FieldDescription> userParameters)
        {
            throw new NotImplementedException();
        }

        public StyroCommand CreateSelectCommand(StyroCommand script, FieldDescription idDescription, List<FieldDescription> userParameters)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<object, string>> SelectProcess(DbReader<StyroReader> reader)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, Type> GetFieldsDescription()
        {
            throw new NotImplementedException();
        }

        public StyroCommand SetKeytoCommand(StyroCommand command, object key)
        {          
            ((IStyroCommandSetId< TKey>)command).SetKey((TKey)key);
            return command;
        }

        public FieldDescription GetKeyDescription()
        {
            throw new NotImplementedException();
        }
    }
}
