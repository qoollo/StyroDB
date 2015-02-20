using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyroDB.Adapter.StyroClient;
using StyroDB.Tests.TestData;

namespace StyroDB.Tests.AdapterTests
{
    [TestClass]
    public class DataReaderTests
    {
        [TestMethod]
        public void GetValueByName_ReferenceType_ReturnsValue()
        {
            var collection = new List<ValueTestClass>()
            {
                new ValueTestClass(0, "string0"),
                new ValueTestClass(1, "string1"),
                new ValueTestClass(2, "string2"),
                new ValueTestClass(3, "string3")
            };
            var resultCollection = new List<ValueTestClass>();
            var dr = new StyroDataReader(collection);

            while (dr.Read())
            {
                resultCollection.Add(new ValueTestClass((int) dr.GetValue("Number"), (string) dr.GetValue("Text")));
            }
            foreach (var item in resultCollection)
            {
                collection.Any(p => p.Number == item.Number && p.Text == item.Text).ShouldBeTrue();
            }
        }

        [TestMethod]
        public void GetValueByOrdinal_ValueType_ReturnsValue()
        {
            var collection = new List<object>() { 0, 1, 2, 3, 4 };
            var resultCollection = new List<object>();
            var dr = new StyroDataReader(collection);

            while (dr.Read())
            {
                resultCollection.Add(dr.GetValue(0));
            }
            foreach (var item in resultCollection)
            {
                collection.Any(p => (int)p == (int)item).ShouldBeTrue();
            }
        }

        [TestMethod]
        public void GetValueByOrdinal_ReferenceType_ReturnsValue()
        {
            var collection = new  List<ValueTestClass>()
            {
                new ValueTestClass(0, "string0"),
                new ValueTestClass(1, "string1"),
                new ValueTestClass(2, "string2"),
                new ValueTestClass(3, "string3")
            };
            var resultCollection = new List<ValueTestClass>();
            var dr = new StyroDataReader(collection);

            while (dr.Read())
            {
                resultCollection.Add(new ValueTestClass((int)dr.GetValue(0), (string)dr.GetValue(1)));
            }
            foreach (var item in resultCollection)
            {
                collection.Any(p => p.Number == item.Number && p.Text == item.Text).ShouldBeTrue();
            }
        }

        [TestMethod]
        public void GetValue_NoValues_ReturnsNull()
        {
            var collection = new  List<object>();
            var dr = new StyroDataReader(collection);

            dr.GetValue(0);
        }


    }
}
