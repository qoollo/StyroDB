using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.Adapter.Internal
{
    internal class MetaData<TKey>
    {
        public TKey Key { get; set; }
        public bool IsLocal { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DeleteTime { get; set; }

        public MetaData(TKey key)
        {
            Key = key;            
        }

    }
}
