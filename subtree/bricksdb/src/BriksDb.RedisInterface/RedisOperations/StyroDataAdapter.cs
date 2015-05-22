using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qoollo.Client.Request;
using StyroDB.InMemrory;

namespace BricksDb.RedisInterface.RedisOperations
{
    class StyroDataAdapter:IDataAdapter
    {
        private readonly IMemoryTable<string, string> _table;
        private readonly RequestDescription _result;

        public StyroDataAdapter(IMemoryTable<string, string> table)
        {
            _table = table;
            _result = new RequestDescription();
        }

        public string Read(string key, out RequestDescription result)
        {
            string res;
            _table.Read(key, out res);
            result = _result;
            return res;
        }

        public RequestDescription Create(string key, string value)
        {
            _table.Write(key, value);
            return _result;
        }
    }
}
