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

        public int Number { get; set; }
        public string Text { get; set; }
    }
}
