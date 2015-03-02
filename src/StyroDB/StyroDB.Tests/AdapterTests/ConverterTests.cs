using System;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            {
                new ValueTestClass(0, "str0",new DateTime(2000,1,1), new NestedClass(1.0)),
                new ValueTestClass(1, "str1",new DateTime(2000,2,3), new NestedClass(5.0)),
                new ValueTestClass(2, "str2",new DateTime(2001,5,1), new NestedClass(3.4)),
                new ValueTestClass(3, "str1",new DateTime(2001,2,1), new NestedClass(1.0)),
                new ValueTestClass(4, "str1",new DateTime(2002,1,1), new NestedClass(6.0)),
                new ValueTestClass(5, "gtr0",new DateTime(2002,1,1), new NestedClass(4.0)),
                new ValueTestClass(1, "shr4",new DateTime(2005,2,3), new NestedClass(4.0)),
                new ValueTestClass(4, "sdr2",new DateTime(2007,5,1), new NestedClass(6.4)),
                new ValueTestClass(2, "sdr1",new DateTime(2005,2,1), new NestedClass(1.0)),
                new ValueTestClass(4, "str1",new DateTime(2000,1,1), new NestedClass(2.0))
            };

        [TestMethod]
        public void GetQueryFunc_IntArgumentWhere_ReturnFunc()
        {
            var jsonQuery = "{\"Where\":\"Number==2\"}";
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);
        }

        [TestMethod]
        public void GetQueryFunc_StrArgumentWhere_ReturnFunc()
        {
            var jsonQuery = "{\"Where\":\"Text=='str0' \"}";
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);
        }

        [TestMethod]
        public void GetQueryFunc_TwoArgumentWhere_ReturnFunc()
        {
            var jsonQuery = "{\"Where\":\"Number==1 and Text=='str1' \"}";
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);
        }

        [TestMethod]
        public void GetQueryFunc_WhereOrderBy_ReturnFunc()
        {
            var jsonQuery = "{\"Where\":\"Text == 'str1' \", \"OrderBy\":\"Number\"}";
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);
        }

        [TestMethod]
        public void GetQueryFunc_WhereWithMethodCall_ReturnFunc()
        {
            var jsonQuery = "{\"Where\":\"Text.Contains('1') \"}";
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);
        }

        [TestMethod]
        public void GetQueryFunc_WhereComplexTypes_ReturnFunc()
        {
            var jsonQuery = String.Format("{{\"Where\":\"Inner.Science==1.0 and Date > Convert.ToDateTime('{0}')\"}}", new DateTime(2000, 5, 5));
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);
        }

        [TestMethod]
        public void GetQueryFunc_MultipleCalls_ReturnFunc()
        {
            var jsonQuery = "{\"Where\":\"Text=='str1'\", \"OrderBy\":\"Number\", \"Skip\":\"1\", \"Take\":\"2\"}";
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);
        }

        [TestMethod]
        public void GetQueryFunc_MultipleCallsB_ReturnFunc()
        {
            var jsonQuery = "{\"Where\":\"Number == 1\", \"Where\":\"Text=='str1'\"}";
            var func = QueryConverter.GetQueryFunc<ValueTestClass>(jsonQuery);
            var result = func(testCollection);
        }
    }
}
