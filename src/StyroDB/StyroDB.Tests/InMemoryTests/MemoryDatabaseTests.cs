using System;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyroDB.InMemrory;
using StyroDB.InMemrory.Exceptions;
using StyroDB.Tests.TestData;

namespace StyroDB.Tests.InMemoryTests
{
    [TestClass]
    public class MemoryDatabaseTests
    {
        private MemoryDatabase _db;

        [TestInitialize]
        public void SetUp()
        {
            _db = new MemoryDatabase();
        }

        [TestMethod]
        public void CreateTable_TableNotExistYet_ReturnsTable()
        {
            var tableName = "table";
            var table = _db.CreateTable<int, ValueTestClass>(tableName);
            table.ShouldNotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(TableAlreadyExistException))]
        public void CreateTable_TableAlreadyExist_ThrowException()
        {
            var tableName = "table";
            var tableA = _db.CreateTable<int, ValueTestClass>(tableName);
            tableA.ShouldNotBeNull();

            _db.CreateTable<int, ValueTestClass>(tableName);
        }

        [TestMethod]
        public void GetTable_TableExist_ReturnsTable()
        {
            var tableName = "table";
            _db.CreateTable<int, ValueTestClass>(tableName);

            var table = _db.GetTable<int, ValueTestClass>(tableName);
            table.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetTable_TableNotExist_ReturnNull()
        {
            var tableName = "table";

            var table = _db.GetTable<int, ValueTestClass>(tableName);
            table.ShouldBeNull();
        }

        [TestMethod]
        public void DeleteTable_TableExist_ProcessOk()
        {
            var tableName = "table";
            _db.CreateTable<int, ValueTestClass>(tableName);

            _db.DeleteTable(tableName);
        }

        [TestMethod]
        public void DeleteTable_TableNotExist_ProcessOk()
        {
            var tableName = "table";

            _db.DeleteTable(tableName);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void WriteToDisposedTable_ThrowException()
        {
            var tableName = "table";
            var key = 0;
            var value = new ValueTestClass();
            var table = _db.CreateTable<int, ValueTestClass>(tableName);

            _db.Dispose();
            table.Write(key,value);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ReadFromDisposedTable_ThrowException()
        {
            var tableName = "table";
            var key = 0;
            var value = new ValueTestClass();
            var table = _db.CreateTable<int, ValueTestClass>(tableName);

            table.Write(key, value);
            _db.Dispose();

            table.Read(key);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CreateTable_DatabaseDisposed_ThrowException()
        {
            _db.Dispose();

            var tableName = "table";
            _db.CreateTable<int, ValueTestClass>(tableName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidTableTypeException))]
        public void GetTable_InvalidCast_ThrowException()
        {
            var tableName = "table";
            _db.CreateTable<int, int>(tableName);

            var table = _db.GetTable<int, ValueTestClass>(tableName);
        }
    }
}
