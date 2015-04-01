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
    public class WrapperTest
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
                new StyroCommandBuilder<int, int>(TableName).ExecuteNonQuery(
                    connection => connection.CreateTable<int, int>(TableName)));
        }

        [TestMethod]
        public void StyroDbModule_CreateTable_CreateAndCheckTable()
        {
            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, int>(TableName)
                .ExecuteNonQuery(connection => connection.CreateTable<int, int>(TableName + TableName)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, int>(TableName)
                .ExecuteNonQuery(connection => connection.GetTable<int, int>(TableName + TableName)
                    .ShouldNotBeNull()))
                .IsError.ShouldBeFalse();
        }


        [TestMethod]
        public void StyroMetaDataCommandCreator_InitMetaDataDb_OnlyOneTable()
        {
            _impl.ExecuteNonQuery(_meta.InitMetaDataDb("blabla"))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(
                new StyroCommandBuilder<int, int>(TableName).
                    ExecuteNonQuery(connection => connection.Tables.Count.ShouldBeEqualTo(1)))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_CreateMetaData_DataExists_SetIsLocalField()
        {
            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, int>(TableName).
                ExecuteNonQuery(table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, ValueWrapper<int, int>>(TableName)
                .ExecuteNonQuery(table => table.Read(1).StyroMetaData.IsLocal.ShouldBeFalse()))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(true), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, ValueWrapper<int, int>>(TableName)
                .ExecuteNonQuery(table => table.Read(1).StyroMetaData.IsLocal.ShouldBeTrue()))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_DeleteMetaData_RemoveFromStyro()
        {
            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, int>(TableName)
                .ExecuteNonQuery(table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.DeleteMetaData(), 1))
                .IsError.ShouldBeFalse();

            ValueWrapper<int, int> result;
            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, ValueWrapper<int, int>>(TableName)
                .ExecuteNonQuery(table => table.Read(1, out result).ShouldBeFalse()))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_SetDataDeleted_IsDeleteFieldIsTrue()
        {
            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, int>(TableName)
                .ExecuteNonQuery(table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, ValueWrapper<int, int>>(TableName)
                .ExecuteNonQuery(table => table.Read(1).StyroMetaData.IsDelete.ShouldBeFalse()))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.SetDataDeleted(), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, ValueWrapper<int, int>>(TableName)
                .ExecuteNonQuery(table => table.Read(1).StyroMetaData.IsDelete.ShouldBeTrue()))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_SetDataNotDeleted_IsDeleteFieldIsFalse()
        {
            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, int>(TableName)
                .ExecuteNonQuery(table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, ValueWrapper<int, int>>(TableName)
                .ExecuteNonQuery(table => table.Read(1).StyroMetaData.IsDelete.ShouldBeFalse()))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.SetDataDeleted(), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, ValueWrapper<int, int>>(TableName)
                .ExecuteNonQuery(table => table.Read(1).StyroMetaData.IsDelete.ShouldBeTrue()))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.SetDataNotDeleted(), 1))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, ValueWrapper<int, int>>(TableName)
                .ExecuteNonQuery(table => table.Read(1).StyroMetaData.IsDelete.ShouldBeFalse()))
                .IsError.ShouldBeFalse();
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_ReadMetaData_ReadNotNull()
        {
            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, int>(TableName)
                .ExecuteNonQuery(table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            var reader = _impl.CreateReader( _meta.SetKeytoCommand(
                    _meta.ReadMetaData(new StyroCommandBuilder<int, int>(TableName).ReadCommand()), 1))
                .ShouldNotBeNull();          
  
            reader.Start();
            reader.IsCanRead.ShouldBeTrue();
            reader.GetValue(0).ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_ReadWithDeleteAndLocal_CreateReader_ReturnValue()
        {
            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, int>(TableName)
                .ExecuteNonQuery(table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            var command = new StyroCommandBuilder<int, int>(TableName).ReadAllDataCommand();
            command = _meta.ReadWithDeleteAndLocal(command, false, false);

            var reader = _impl.CreateReader(command);
            reader.Start();

            reader.IsCanRead.ShouldBeTrue();
            reader.GetValue(0).ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void StyroMetaDataCommandCreator_ReadWithDeleteAndLocal_CreateReader_NoData_ReturnNoValue()
        {
            _impl.ExecuteNonQuery(new StyroCommandBuilder<int, int>(TableName)
                .ExecuteNonQuery(table => table.Write(1, 1)))
                .IsError.ShouldBeFalse();

            _impl.ExecuteNonQuery(_meta.SetKeytoCommand(_meta.CreateMetaData(false), 1))
                .IsError.ShouldBeFalse();

            var command = new StyroCommandBuilder<int, int>(TableName).ReadAllDataCommand();
            command = _meta.ReadWithDeleteAndLocal(command, true, false);

            var reader = _impl.CreateReader(command);
            reader.Start();

            reader.IsCanRead.ShouldBeFalse();
        }
    }
}
