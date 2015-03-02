using StyroDB.Adapter.Internal;

namespace StyroDB.Adapter.Wrappers
{
    internal class ValueWrapper<TKey, TValue>
    {
        public ValueWrapper(TKey key, TValue value)
        {
            Value = value;
            StyroMetaData = new StyroMetaData<TKey>(key);
        }

        public TValue Value { get; private set; }
        public StyroMetaData<TKey> StyroMetaData { get; private set; } 
    }
}