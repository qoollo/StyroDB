using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyroDB.InMemrory;
using StyroDB.Tests.InMemoryTests.Helper;
using StyroDB.Tests.TestData;

namespace StyroDB.Tests.InMemoryTests
{
    [TestClass]
    public class MemoryTableTests
    {
        private const string TableName = "test_table";
        private MemoryTableControlLock<int, ValueTestClass> _table;
        
            
        [TestInitialize]
        public void SetUp()
        {
            _table = new MemoryTableControlLock<int, ValueTestClass>(TableName);
            //MemoryTableHelper.FillTable(table);
            //MemoryTableHelper.LockOnRead(table);
        }

        #region Read Tests
        [TestMethod]
        public void Read_NoLocks_ReturnsValue()
        {
            int key = 0;
            var value = new ValueTestClass();
            _table.Write(key, value);

            var readValue = _table.Read(key);
            readValue.ShouldBeEqualTo(value);
        }

        [TestMethod]
        public void Read_WriteLockEndsBeforeTiemout_ReturnsValue()
        {
            int timeout = 1000;
            int key = 0;
            var value = new ValueTestClass();
            _table.Write(key, value);

            _table.EnterWriteLock(timeout);

            var readValue = _table.Read(key, timeout * 3);
            readValue.ShouldBeEqualTo(value);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Read_NoLocksAndNoValue_ThrowsException()
        {
            int key = 0;
            _table.Read(key);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void Read_WriteLockAndTimeout_ThrowsException()
        {
            int timeout = 1000;
            int key = 0;
            var value = new ValueTestClass();
            _table.Write(key, value);

            _table.EnterWriteLock(timeout * 2);
            _table.Read(key, timeout / 2);
        } 
        #endregion

        #region Write Tests
        
        [TestMethod]
        public void Write_NoLocks_ProcessOk()
        {

        }

        [TestMethod]
        public void Write_WriteLockEndsBeforeTimeout_ProcessOk()
        {

        }

        [TestMethod]
        public void Write_WriteLockAndTimeout_ThrowsException()
        {

        }

        [TestMethod]
        public void Write_ReadLockEndsBeforeTiemout_ProcessOk()
        {

        }

        [TestMethod]
        public void Write_ReadLockAndTimeout_ThrowsException()
        {

        }
        
        #endregion

        #region Update Tests

        [TestMethod]
        public void Update_NoLocks_ProcessOk()
        {

        }

        [TestMethod]
        public void Update_WriteLockAndTimeout_ThrowsException()
        {

        }
        
        #endregion

        #region Delete Tests

        [TestMethod]
        public void Delete_NoLocks_ProcessOk()
        {

        }

        [TestMethod]
        public void Delete_NoLocksAndNoValue_ThrowsException()
        {

        }

        #endregion

    }
}
