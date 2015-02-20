using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.Adapter.StyroClient.Commands
{
    internal class StyroCommandReadWithoutId<TKey, TValue>:StyroCommand
    {
        private TKey _key;

        public StyroCommandReadWithoutId(string tableName)
            : base(tableName)
        {            
        }

        public void SetId(TKey key)
        {
            _key = key;
        }

        internal override void ExecuteNonQueryInner(StyroConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        internal override IEnumerable<object> ExecuteReaderInner(StyroConnection connection, string tableName)
        {
            var list = new List<object> ();
            var table = connection.GetTable<TKey, TValue>(tableName);
            try
            {
                var value = table.Read(_key);            
                list.Add(value);
            }
            catch (KeyNotFoundException)
            {                
            }            

            return list;
        }
    }
}
