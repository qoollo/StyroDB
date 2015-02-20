using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyroDB.Adapter.Internal;
using StyroDB.Adapter.StyroClient;
using StyroDB.Adapter.StyroClient.Commands;
using StyroDB.Adapter.Wrappers;

namespace StyroDB.Tests.AdapterTests
{
    [TestClass]
    public class WarpperTest
    {
        private const string TableName = "TestTable";
        private StyroDbModule _impl;
        private StyroMetaDataCommandCreator<int, int> _meta;

        [TestInitialize]
        public void Initialize()
        {
            _impl = new StyroDbModule(new StyroConnectionParams(), 1, 100);
            _impl.Build();
            _impl.Start();

            _meta = new StyroMetaDataCommandCreator<int, int>();
            _meta.SetTableName(new List<string>() {TableName});

            _impl.ExecuteNonQuery(
                new StyroCommandDatabaseAction(connection => connection.CreateTable<int, int>(TableName)));
        }

        [TestMethod]
        public void StyroDbModule_CreateTable_CreateAndCheckTable()
        {
            _impl.ExecuteNonQuery(
                new StyroCommandDatabaseAction(connection => connection.CreateTable<int, int>(TableName + TableName)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(
                new StyroCommandDatabaseAction(connection =>
                    connection.GetTable<int, int>(TableName + TableName).ShouldNotBeNull()))
                .IsError.ShouldBeFalse();
        }


        [TestMethod]
        public void StyroMetaDataCommandCreator_InitMetaDataDb_OnlyOneTable()
        {
            _impl.ExecuteNonQuery(_meta.InitMetaDataDb("blabla"))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(
                new StyroCommandDatabaseAction(connection => connection.Tables.Count.ShouldBeEqualTo(1)))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_CreateMetaData_DataExists_SetIsLocalField()
        {
            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, int>(TableName, table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, ValueWrapper<int, int>>(TableName,
                table => table.Read(1).MetaData.IsLocal.ShouldBeFalse()))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(true), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, ValueWrapper<int, int>>(TableName,
                table => table.Read(1).MetaData.IsLocal.ShouldBeTrue()))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_DeleteMetaData_RemoveFromStyro()
        {
            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, int>(TableName, table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.DeleteMetaData(), 1))
                .IsError.ShouldBeFalse();

            ValueWrapper<int, int> result;
            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, ValueWrapper<int, int>>(TableName,
                table => table.Read(1, out result).ShouldBeFalse()))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_SetDataDeleted_IsDeleteFieldIsTrue()
        {
            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, int>(TableName, table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, ValueWrapper<int, int>>(TableName,
                table => table.Read(1).MetaData.IsDelete.ShouldBeFalse()))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.SetDataDeleted(), 1))
                .IsError.ShouldBeFalse();
           
            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, ValueWrapper<int, int>>(TableName,
                table => table.Read(1).MetaData.IsDelete.ShouldBeTrue()))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_SetDataNotDeleted_IsDeleteFieldIsFalse()
        {
            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, int>(TableName, table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, ValueWrapper<int, int>>(TableName,
                table => table.Read(1).MetaData.IsDelete.ShouldBeFalse()))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.SetDataDeleted(), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, ValueWrapper<int, int>>(TableName,
                table => table.Read(1).MetaData.IsDelete.ShouldBeTrue()))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.SetDataNotDeleted(), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, ValueWrapper<int, int>>(TableName,
                table => table.Read(1).MetaData.IsDelete.ShouldBeFalse()))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_SetDataNotDeleted_IsDeleteFieldIsFalse2()
        {
            _impl.ExecuteNonQuery(new StyroCommandNonQuery<int, int>(TableName, table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            var reader = _impl.CreateReader(
                _meta.SetKeytoCommand(
                    _meta.ReadMetaData(new StyroCommandReadWithId<int, int>(TableName)), 1)).ShouldNotBeNull();

            //TODO check reader

        }
    }
}
