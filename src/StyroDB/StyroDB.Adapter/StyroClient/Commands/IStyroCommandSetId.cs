using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.Adapter.StyroClient.Commands
{
    interface IStyroCommandSetId<in TKey>
    {
        void SetKey(TKey key);
    }
}
