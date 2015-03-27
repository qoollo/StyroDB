using StyroDB.Adapter.Internal;

namespace StyroDB.Adapter.Wrappers
{
    internal class ValueWrapper<TKey, TValue>
    {
        public ValueWrapper(TKey key, TValue value)
        {
            Value = value;
            MetaData = new MetaData<TKey>(key);
        }

        public TValue Value { get; private set; }
        public MetaData<TKey> MetaData { get; private set; } 
    }
}