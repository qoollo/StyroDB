using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StyroDB.InMemrory;

namespace StyroDB.Tests.InMemoryTests.Helper
{
    class MemoryTableControlLock<TKey, TValue> : MemoryTable<TKey, TValue>
    {
        public MemoryTableControlLock(string tableName) : base(tableName) { }

        public void EnterWriteLock(int timeout)
        {
            /*var t = Task.Run(async delegate
            {
                _cacheLock.EnterWriteLock();
                await Task.Delay(timeout);
                _cacheLock.ExitWriteLock();
            });*/

            Task.Factory.StartNew(() =>
            {
                _cacheLock.EnterWriteLock();
                Thread.Sleep(timeout);
                _cacheLock.ExitWriteLock();
            });
            
            Thread.Sleep(100); // time for the task to start new thread
        }

        public void ExitWriteLock()
        {
            _cacheLock.ExitWriteLock();
        }

        public void EnterReadLock(int timeout)
        {
            var t = Task.Run(async delegate
            {
                _cacheLock.EnterReadLock();
                await Task.Delay(timeout);
                _cacheLock.ExitReadLock();
            });

            Thread.Sleep(100); // time for the task to start new thread
        }

        public void ExitReadLock()
        {
            _cacheLock.ExitReadLock();
        }
    }
}
