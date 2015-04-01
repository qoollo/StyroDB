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
            return
                new StyroCommandBuilder<TKey, ValueWrapper<TKey, TValue>>(_tableName).ExecuteNonQuery(
                    (IMemoryTable<TKey, ValueWrapper<TKey, TValue>> tableName) => { });
        }

        public StyroCommand CreateMetaData(bool remote)
        {
            return new StyroCommandBuilder<TKey, ValueWrapper<TKey, TValue>>(_tableName).ExecuteNonQuery(
                (key, table) =>
                {
                    var value = table.Read(key);
                    value.StyroMetaData.IsLocal = remote;
                    table.Write(key, value);
                });
        }

        public StyroCommand DeleteMetaData()
        {
            return new StyroCommandBuilder<TKey, ValueWrapper<TKey, TValue>>(_tableName).ExecuteNonQuery(
                (key, table) => table.Delete(key));            
        }

        public StyroCommand UpdateMetaData(bool local)
        {
            return new StyroCommandBuilder<TKey, ValueWrapper<TKey, TValue>>(_tableName).ExecuteNonQuery(
                (key, table) =>
                {
                    var value = table.Read(key);
                    value.StyroMetaData.IsLocal = local;
                    table.Write(key, value);
                });
        }

        public StyroCommand SetDataDeleted()
        {
            return new StyroCommandBuilder<TKey, ValueWrapper<TKey, TValue>>(_tableName).ExecuteNonQuery(
                (key, table) =>
                {
                    var value = table.Read(key);
                    value.StyroMetaData.IsDelete = true;
                    value.StyroMetaData.DeleteTime = DateTime.Now;
                    table.Write(key, value);
                });
        }

        public StyroCommand SetDataNotDeleted()
        {
            return new StyroCommandBuilder<TKey, ValueWrapper<TKey, TValue>>(_tableName).ExecuteNonQuery(
                (key, table) =>
                {
                    var value = table.Read(key);
                    value.StyroMetaData.IsDelete = false;
                    table.Write(key, value);
                });
        }

        public StyroCommand ReadMetaData(StyroCommand userRead)
        {
            return new StyroCommandBuilder<TKey, ValueWrapper<TKey, TValue>>(_tableName).ReadCommand();            
        }

        public Tuple<MetaData, bool> ReadMetaDataFromReader(DbReader<StyroReader> reader, bool readuserId = true)
        {
            var styroMeta = (StyroMetaData<TKey>) reader.GetValue(0);
            var meta = new MetaData(styroMeta.IsLocal, styroMeta.DeleteTime, styroMeta.IsDelete);

            if (readuserId)
                meta.Id = styroMeta.Key;

            return new Tuple<MetaData, bool>(meta, readuserId);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public MetaData ReadMetaFromSearchData(SearchData data)
        {
            
            throw new NotImplementedException();
        }

        public string ReadWithDeleteAndLocal(bool isDelete, bool local)
        {
            return string.Format("ReadWithDeleteAndLocal:{0}:{1}", isDelete, local);
        }

        public StyroCommand ReadWithDelete(StyroCommand userRead, bool isDelete)
        {
            return userRead;
        }

        public StyroCommand ReadWithDeleteAndLocal(StyroCommand userRead, bool isDelete, bool local)
        {
            var builder = new StyroCommandBuilder<TKey, ValueWrapper<TKey, TValue>>(_tableName);
            var command =  builder
                .ExecuteReader(wrappers =>
                {
                    userRead.Connection = builder.Connection;

                    var collection = userRead.ExecuteReaderGetCollection().Cast<ValueWrapper<TKey, TValue>>();
                    collection = collection.Where(x =>
                    {
                        var ret = x.StyroMetaData.IsDelete == isDelete;
                        if (local)
                            ret = ret && x.StyroMetaData.IsLocal;
                        return ret;
                    });
                    return collection;
                });
            return command;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="script"></param>
        /// <param name="idDescription"></param>
        /// <param name="userParameters"></param>
        /// <returns></returns>
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
