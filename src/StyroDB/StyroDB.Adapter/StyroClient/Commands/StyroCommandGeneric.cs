using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.Adapter.StyroClient.Commands
{
    class StyroCommandGeneric<TKey>:StyroCommand, IStyroCommandSetId<TKey>
    {
        public TKey Key { get; private set; }

        protected internal StyroCommandGeneric(string tableName) : base(tableName)
        {
        }

        public void SetKey(TKey key)
        {
            Key = key;
        }
    }
}
