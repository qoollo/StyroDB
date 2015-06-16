using System.Linq;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qoollo.Impl.Common.Data.Support;
using Qoollo.Impl.Common.NetResults;

namespace StyroDB.Tests.AdapterTests
{    
    [TestClass]
    public class StyroWriterTests : TestBase
    {
        [TestMethod]
        public void DbModule_ProcessSync_WriteValue()
        {
            var data = CreateData(1, 1, OperationName.Create);
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();
            Table.Read(1).Value.ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_WriteDiplicateValue_NoException()
        {
            var data = CreateData(1, 1, OperationName.Create);
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();

            Table.Read(1).Value.ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_WriteAndReadValue()
        {
            var data = CreateData(1, 1, OperationName.Create);
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();

            data = CreateData(1, 100, OperationName.Read);
            data = Channel.ReadOperation(data);

            Provider.DeserializeValue(data.Data).ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_ReadValueWithoutWrite()
        {
            var data = CreateData(1, 100, OperationName.Read);
            data = Channel.ReadOperation(data);

            data.Data.ShouldBeNull();
        }

        [TestMethod]
        public void DbModule_ProcessSync_WriteAndReadCountElements()
        {
            int count = 100;
            for (int i = 0; i < count; i++)
            {
                var data = CreateData(i, i, OperationName.Create);
                Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();
            }

            for (int i = 0; i < count; i++)
            {
                var data = CreateData(i, i+100, OperationName.Read);
                data = Channel.ReadOperation(data);

                Provider.DeserializeValue(data.Data).ShouldBeEqualTo(i);
            }


        }

        [TestMethod]
        public void DbModule_ProcessSync_UpdateWithWrite_OnlyOneValue()
        {
            var data = CreateData(1, 1, OperationName.Create);
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();
            Table.Read(1).Value.ShouldBeEqualTo(1);

            data = CreateData(1, 10, OperationName.Update);
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();

            Table.Read(1).Value.ShouldBeEqualTo(10);

            Table.Query(x => x).Count().ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_UpdateWithoutWrite()
        {
            var data = CreateData(1, 10, OperationName.Update);
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();

            Table.Read(1).Value.ShouldBeEqualTo(10);

            Table.Query(x => x).Count().ShouldBeEqualTo(1);
        }

        [TestMethod]
        public void DbModule_ProcessSync_DeleteWithWrite()
        {
            var data = CreateData(1, 1, OperationName.Create);
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();

            data = CreateData(1, 100, OperationName.Delete);
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();

            data = CreateData(1, 100, OperationName.Read);
            data = Channel.ReadOperation(data);

            data.Data.ShouldBeNull();
        }

        [TestMethod]
        public void DbModule_ProcessSync_DeleteWithoutWrite()
        {
            var data = CreateData(1, 100, OperationName.Delete);
            Channel.ProcessSync(data).ShouldBeOfType<SuccessResult>();

            data = CreateData(1, 100, OperationName.Read);
            data = Channel.ReadOperation(data);

            data.Data.ShouldBeNull();
        }

        [TestMethod]
        public void DbModule_ProcessSync_Select()
        {   

        }
    }
}
