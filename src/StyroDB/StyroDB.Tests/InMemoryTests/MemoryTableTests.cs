using System;
using System.Collections.Generic;
using System.Linq;
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
        public void Read_NoLocksAndNoValue_ReturnFalse()
        {
            int key = 0;
            ValueTestClass readValue;
            var exist = _table.Read(key, out readValue);
            exist.ShouldBeEqualTo(false);
            readValue.ShouldBeEqualTo(null);
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
        public void Write_NoLocks_AddValue()
        {
            int key = 0;
            var value = new ValueTestClass();
            _table.Write(key, value);

            var readValue = _table.Read(key);
            readValue.ShouldBeEqualTo(value);
        }

        [TestMethod]
        public void Write_NoLocks_UpdateValue()
        {
            int key = 0;
            var valueA = new ValueTestClass();
            var valueB = new ValueTestClass();
            _table.Write(key, valueA);
            _table.Write(key, valueB);

            var readValue = _table.Read(key);
            readValue.ShouldBeEqualTo(valueB);
        }

        [TestMethod]
        public void Write_WriteLockEndsBeforeTimeout_ProcessOk()
        {
            int timeout = 1000;
            int key = 0;
            var value = new ValueTestClass();
            _table.EnterWriteLock(timeout);
            _table.Write(key, value, 2 * timeout);

            var readValue = _table.Read(key, 3 * timeout);
            readValue.ShouldBeEqualTo(value);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void Write_WriteLockAndTimeout_ThrowsException()
        {
            int timeout = 1000;
            int key = 0;
            var value = new ValueTestClass();
            _table.EnterWriteLock(2 * timeout);
            _table.Write(key, value, timeout);

        }
        
        #endregion

        #region Delete Tests

        [TestMethod]
        public void Delete_NoLocks_ProcessOk()
        {
            int key = 0;
            var value = new ValueTestClass();
            _table.Write(key, value);

            var existDel = _table.Delete(key);
            existDel.ShouldBeEqualTo(true);

            ValueTestClass readValue;
            var exist = _table.Read(key, out readValue);
            exist.ShouldBeEqualTo(false);
            readValue.ShouldBeEqualTo(null);
        }

        [TestMethod]
        public void Delete_NoLocksAndNoValue_ThrowsException()
        {
            int key = 0;
            var existDel = _table.Delete(key);
            existDel.ShouldBeEqualTo(false);

            ValueTestClass readValue;
            var exist = _table.Read(key, out readValue);
            exist.ShouldBeEqualTo(false);
            readValue.ShouldBeEqualTo(null);
        }

        #endregion

        #region Select

        [TestMethod]
        public void Select_NoLocks_ReturnEnumerable()
        {
            var dict = new List<KeyValuePair<int, ValueTestClass>>()
            {
                new KeyValuePair<int, ValueTestClass>(0, new ValueTestClass(0, "string1")),
                new KeyValuePair<int, ValueTestClass>(1, new ValueTestClass(1, "string2")),
                new KeyValuePair<int, ValueTestClass>(2, new ValueTestClass(2, "string1"))
            };
            foreach (var item in dict)
            {
                _table.Write(item.Key, item.Value);
            }

            Func<IEnumerable<ValueTestClass>, IEnumerable<ValueTestClass>> func =
                (x) => x.Where(p => p.Text.Contains("1"));
            var resultQ = _table.Query(func);
        }

        #endregion
    }
}
