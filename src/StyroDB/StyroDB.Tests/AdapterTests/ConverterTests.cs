using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using StyroDB.Adapter.Converter;
using StyroDB.Tests.TestData;

namespace StyroDB.Tests.AdapterTests
{
    /// <summary>
    /// Summary description for ConverterTests
    /// </summary>
    [TestClass]
    public class ConverterTests
    {
        private List<ValueTestClass> testCollection = new List<ValueTestClass>()
            { // you can only add new items to the end of this list, because all items are already used by their indexes
                new ValueTestClass(0, "str0",new DateTime(2000,1,1), new NestedClass(1.0)), // 0
                new ValueTestClass(1, "str1",new DateTime(2000,2,3), new NestedClass(5.0)), // 1
                new ValueTestClass(2, "str2",new DateTime(2001,5,1), new NestedClass(3.4)), // 2
                new ValueTestClass(3, "str1",new DateTime(2001,2,1), new NestedClass(1.0)), // 3
                new ValueTestClass(4, "str1",new DateTime(2002,1,1), new NestedClass(6.0)), // 4
                new ValueTestClass(5, "gtr0",new DateTime(2002,1,1), new NestedClass(4.0)), // 5
                new ValueTestClass(1, "shr4",new DateTime(2005,2,3), new NestedClass(4.0)), // 6
                new ValueTestClass(4, "sdr2",new DateTime(2007,5,1), new NestedClass(6.4)), // 7
                new ValueTestClass(2, "sdr1",new DateTime(2005,2,1), new NestedClass(1.0)), // 8
                new ValueTestClass(4, "str1",new DateTime(2000,1,1), new NestedClass(2.0))  // 9
            };

        [TestMethod]
        public void GetQueryFunc_IntArgumentWhere_ReturnFunc()
        {
            var shouldBe = new List<ValueTestClass>()
            {
                testCollection[2],
                testCollection[8],
            };
            var jsonQuery = "{Where:Number==2}";
            
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);

            CompareKeyValCollections(result, shouldBe).ShouldBeTrue();
        }

        [TestMethod]
        public void GetQueryFunc_StrArgumentWhere_ReturnFunc()
        {
            var shouldBe = new List<ValueTestClass>()
            {
                testCollection[0]
            };
            var jsonQuery = "{Where:Text==\"str0\"}";
            
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);

            CompareKeyValCollections(result, shouldBe).ShouldBeTrue();
        }

        [TestMethod]
        public void GetQueryFunc_TwoArgumentWhere_ReturnFunc()
        {
            var shouldBe = new List<ValueTestClass>()
            {
                testCollection[1]
            };
            var jsonQuery = "{Where:Number==1 and Text==\"str1\"}";
            
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);

            CompareKeyValCollections(result, shouldBe).ShouldBeTrue();
        }

        [TestMethod]
        public void GetQueryFunc_WhereOrderBy_ReturnFunc()
        {
            var shouldBe = new List<ValueTestClass>()
            {
                testCollection[1],
                testCollection[3],
                testCollection[4],
                testCollection[9]
            };
            var jsonQuery = "{Where:Text == \"str1\"}, {OrderBy:Number}";
            
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);

            CompareKeyValCollectionsOrdered(result.ToList(), shouldBe).ShouldBeTrue();
        }

        [TestMethod]
        public void GetQueryFunc_WhereWithMethodCall_ReturnFunc()
        {
            var shouldBe = new List<ValueTestClass>()
            {
                testCollection[1],
                testCollection[3],
                testCollection[4],
                testCollection[8],
                testCollection[9]
            };
            var jsonQuery = "{Where:Text.Contains(\"1\")}";
            
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);

            CompareKeyValCollections(result, shouldBe).ShouldBeTrue();
        }

        [TestMethod]
        public void GetQueryFunc_WhereComplexTypes_ReturnFunc()
        {
            var shouldBe = new List<ValueTestClass>()
            {
                testCollection[3],
                testCollection[8]
            };
            var jsonQuery = String.Format("{{Where:Inner.Science==1.0 and Date > Convert.ToDateTime(\"{0}\")}}",
                new DateTime(2000, 5, 5));
            
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);

            CompareKeyValCollections(result, shouldBe).ShouldBeTrue();
        }

        [TestMethod]
        public void GetQueryFunc_MultipleCalls_ReturnFunc()
        {
            var shouldBe = new List<ValueTestClass>()
            {
                testCollection[3],
                testCollection[4]
            };
            var jsonQuery = "{Where:Text==\"str1\"}, {OrderBy:Number}, {Skip:1}, {Take:2}}";

            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);

            CompareKeyValCollections(result, shouldBe).ShouldBeTrue();
        }

        [TestMethod]
        public void GetQueryFunc_MultipleCallsB_ReturnFunc()
        {
            var shouldBe = new List<ValueTestClass>()
            {
                testCollection[1]
            };
            var jsonQuery = "{Where:Number == 1}, {Where:Text==\"str1\"}";

            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);

            CompareKeyValCollections(result, shouldBe).ShouldBeTrue();
        }

        public bool CompareKeyValCollections<T>(IEnumerable<T> col1, IEnumerable<T> col2)
        {
            if (col1.Count() != col2.Count())
            {
                return false;
            }
            if (col1.Any(item => !col2.Contains(item)))
            {
                return false;
            }
            return true;
        }

        public bool CompareKeyValCollectionsOrdered<T>(List<T> col1, List<T> col2)
        {
            if (col1.Count() != col2.Count())
            {
                return false;
            }
            for (int i = 0; i < col1.Count(); i++)
            {
                if (!col1[i].Equals(col2[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
