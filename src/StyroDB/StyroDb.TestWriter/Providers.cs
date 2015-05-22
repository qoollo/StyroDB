using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qoollo.Client.Support;

namespace StyroDb.TestWriter
{
    public class IntDataProvider : CommonDataProvider<int, int>
    {
        public override string CalculateHashFromKey(int key)
        {
            return key.GetHashCode().ToString(CultureInfo.InvariantCulture);
        }
    }

    public class StringDataProvider : CommonDataProvider<string, String>
    {
        public override string CalculateHashFromKey(string key)
        {
            return key;
        }
    }

    public class LongStringProvider : CommonDataProvider<long, string>
    {
        public override string CalculateHashFromKey(long key)
        {
            return key.ToString();
        }
    }
}
