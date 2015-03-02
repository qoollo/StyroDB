using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyroDB.Adapter.Internal;
using StyroDB.Adapter.Wrappers;

namespace StyroDB.Adapter.StyroClient
{
    class StyroDataReaderWithWrapper<TKey, TValue>:StyroDataReader
    {
        internal StyroDataReaderWithWrapper(IEnumerable<object> values) : base(values)
        {
        }

        internal override IEnumerable<Property> GetValueProperies(object value)
        {
            var obj = (ValueWrapper<TKey, TValue>) value;

            var list = base.GetValueProperies(obj.Value).ToList();
            list.ForEach(x => x.Ordinal++);
            list.Insert(0, new Property("StyroMetaData", 0, typeof (StyroMetaData<TKey>), obj.StyroMetaData));
            return list;
        }
    }
}
