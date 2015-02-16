﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyroDB.Adapter.StyroClient;
using StyroDB.Adapter.StyroClient.Commands;
using StyroDB.InMemrory;
using StyroDB.Tests.Helper;

namespace StyroDB.Tests.AdapterTests
{
    [TestClass]
    public class ConnectionTests
    {
        private const string TableName = "testTable";
        private TestMemoryDatabase _db;
        private StyroConnection _connection;

        [TestInitialize]
        public void Initialize()
        {
            _db = new TestMemoryDatabase(new MemoryDatabase());
            _connection = new StyroConnection(_db);

            _db.CreateTable<int, int>(TableName);

        }

        [TestMethod]
        public void StyroConnection_GetTable_NullExpected()
        {
            _connection.GetTable<int, int>(TableName+"1").ShouldBeNull();
        }

        [TestMethod]
        public void StyroConnection_GetTable_SaveTable()
        {            
            _connection.GetTable<int, int>(TableName);
            _db.GetCount.ShouldBeEqualTo(1);

            _connection.GetTable<int, int>(TableName);
            _db.GetCount.ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void StyroCommandNonQuery_ExecuteNonQuery_WriteData()
        {
            var command = new StyroCommandNonQuery<int, int>(TableName, table => table.Write(1, 1))
            {
                Connection = _connection
            };

            command.ExecuteNonQuery();
            _db.GetTable<int, int>(TableName).Read(1).ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void StyroCommandNonQuery_ExecuteNonQuery_WriteCountData()
        {
            _connection.GetTable<int, int>(TableName).Write(1, 0);

            var command = new StyroCommandNonQuery<int, int>(TableName, 
                table => table.Write(1, table.Read(1) + 1))
            {
                Connection = _connection
            };

            command.ExecuteNonQuery();
            _db.GetTable<int, int>(TableName).Read(1).ShouldBeEqualTo(1);
            command.ExecuteNonQuery();
            _db.GetTable<int, int>(TableName).Read(1).ShouldBeEqualTo(2);
            command.ExecuteNonQuery();
            _db.GetTable<int, int>(TableName).Read(1).ShouldBeEqualTo(3);
        }

        [TestMethod]
        public void StyroCommandRead_ExecuteReader_ReadSingleData()
        {
            _connection.GetTable<int, int>(TableName).Write(1, 0);

            var command = new StyroCommandRead<int, int>(TableName, 1) {Connection = _connection};
            foreach (int data in command.ExecuteReader())
            {
                data.ShouldBeEqualTo(0);
            }
        }

        [TestMethod]
        public void StyroCommandRead_ExecuteReader_ReadSingleDataNotExist()
        {
            _connection.GetTable<int, int>(TableName).Write(1, 0);

            var command = new StyroCommandRead<int, int>(TableName, 2) { Connection = _connection };
            command.ExecuteReader().Count().ShouldBeEqualTo(0);
        }

        [TestMethod]
        public void StyroCommandQuery_ExecuteReader_ReadSliceOfData()
        {
            _connection.GetTable<int, int>(TableName).Write(1, 1);
            _connection.GetTable<int, int>(TableName).Write(2, 2);
            _connection.GetTable<int, int>(TableName).Write(3, 3);

            var command = new StyroCommandQuery<int, int>(TableName, table => table.Where(x => x > 1))
            {
                Connection = _connection
            };

            command.ExecuteReader().Count().ShouldBeEqualTo(2);
        }

        [TestMethod]
        public void StyroCommandQuery_ExecuteReader_NoData()
        {
            _connection.GetTable<int, int>(TableName).Write(1, 1);
            _connection.GetTable<int, int>(TableName).Write(2, 2);
            _connection.GetTable<int, int>(TableName).Write(3, 3);

            var command = new StyroCommandQuery<int, int>(TableName, table => table.Where(x => x < 1))
            {
                Connection = _connection
            };

            command.ExecuteReader().Count().ShouldBeEqualTo(0);
        }
    }
}
