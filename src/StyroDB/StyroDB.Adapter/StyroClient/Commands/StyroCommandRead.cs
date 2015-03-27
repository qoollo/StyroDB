using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.Adapter.StyroClient.Commands
{
    public class StyroCommandRead<TKey, TValue>:StyroCommand
    {
        private readonly TKey _key;

        public StyroCommandRead(string tableName, TKey key ) : base(tableName)
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
