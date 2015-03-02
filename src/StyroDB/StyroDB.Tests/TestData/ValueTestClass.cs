using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.Tests.TestData
{
    class ValueTestClass
    {
        public ValueTestClass()
        {

        }

        public ValueTestClass(int number, string text)
        {
            Number = number;
            Text = text;
        }

        public ValueTestClass(int number, string text, DateTime date, NestedClass inner)
        {
            Number = number;
            Text = text;
            Date = date;
            Inner = inner;
        }

        public int Number { get; set; }
        public string Text { get; set; }

        public DateTime Date { get; set; }
        public NestedClass Inner { get; set; }
    }

    class NestedClass
    {
        public NestedClass(double science)
        {
            Science = science;
        }

        public double Science { get; set; }
    }
}
