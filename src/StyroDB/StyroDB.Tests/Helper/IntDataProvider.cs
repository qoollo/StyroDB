using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qoollo.Client.Support;

namespace StyroDB.Tests.Helper
{
    class IntDataProvider:CommonDataProvider<int, int>
    {
        public override string CalculateHashFromKey(int key)
        {
            return key.GetHashCode().ToString(CultureInfo.InvariantCulture);
        }
    }
}
