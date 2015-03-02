using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StyroDB.InMemrory;
using System.Linq.Dynamic;

namespace StyroDB.Adapter.Converter
{
    class QueryConverter
    {
        static QueryConverter()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        public static Func<IEnumerable<TValue>, IEnumerable<TValue>> GetQueryFunc<TValue>(string jsonQuery)
        {
            var expr = GetExpressionFromJson(jsonQuery);
            return MakeFunc<TValue>(expr);
        }

        public static List<KeyValuePair<string,string>> GetExpressionFromJson(string jsonQuery)
        {
            var rawObj = JObject.Parse(jsonQuery);
            var jsonList = new List<KeyValuePair<string, string>>();
            foreach (var item in rawObj.Children())
            {
                foreach (var prop in item)
                {
                    var property = prop as JProperty;

                    if (property != null)
                    {
                        jsonList.Add(new KeyValuePair<string, string>(property.Name,
                            property.Value.ToString().Replace("'", @"""")));
                    }

                }
            }
            /*var jsonList =
                rawObj.Children()
                    .Cast<JProperty>()
                    .ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Name, ((string) x.Value).Replace("'",@"""")))
                    .ToList();*/
            return jsonList;
        }

        public static Func<IEnumerable<TValue>, IEnumerable<TValue>> MakeFunc<TValue>(List<KeyValuePair<string, string>> query)
        {
            Func<IEnumerable<TValue>, IEnumerable<TValue>> func =(x) => x.AsQueryable();
            
            foreach (var pair in query)
            {
                func = ExpandFunc(func, pair.Key, pair.Value);
            }
            return func;
        }

        
        private static Func<IEnumerable<TValue>, IEnumerable<TValue>> ExpandFunc<TValue>(
            Func<IEnumerable<TValue>, IEnumerable<TValue>> func, string method, string argument)
        {
            var funcs = new Dictionary<string, Func<IEnumerable<TValue>, IEnumerable<TValue>>>()
            {
                {"Where", x => func(x).AsQueryable().Where(argument)},
                {"OrderBy",x => func(x).AsQueryable().OrderBy(argument)},
                {"Take",x => func(x).Take(Convert.ToInt32(argument))},
                {"Skip",x => func(x).Skip(Convert.ToInt32(argument))}
            };
            return funcs[method];
/*            Func<IEnumerable<TValue>, IEnumerable<TValue>> f = x => x;
            if (method == "Where")
            {
                f = x => func(x).AsQueryable().Where(argument);
            }
            if (method == "OrderBy")
            {
                f = x => func(x).AsQueryable().OrderBy(argument);
            }
            if (method == "Take")
            {
                f = x => func(x).Take(Convert.ToInt32(argument));
            }
            if (method == "Skip")
            {
                f = x => func(x).Skip(Convert.ToInt32(argument));
            }
            return f;*/
        }
    }

}
