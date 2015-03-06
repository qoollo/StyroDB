using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Threading;
using StyroDB.Adapter.Converter.Exceptions;

namespace StyroDB.Adapter.Converter
{
    internal class QueryConverter
    {
        static QueryConverter()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        public static Func<IEnumerable<TValue>, IEnumerable<TValue>> GetQueryFunc<TValue>(string jsonQuery)
        {
            List<KeyValuePair<string, string>> expr = GetExpressionFromJson(jsonQuery);
            return MakeFunc<TValue>(expr);
        }

        public static List<KeyValuePair<string, string>> GetExpressionFromJson(string jsonQuery)
        {
            var jsonList = new List<KeyValuePair<string, string>>();
            var regex = new Regex("{.*?}");
            MatchCollection matches = regex.Matches(jsonQuery);
            foreach (object match in matches)
            {
                string[] keyvalue = match.ToString().Trim(new[] {'{', '}'}).Split(new[] {':'}, 2);
                jsonList.Add(new KeyValuePair<string, string>(keyvalue[0], keyvalue[1]));
            }
            return jsonList;
        }

        public static Func<IEnumerable<TValue>, IEnumerable<TValue>> MakeFunc<TValue>(
            List<KeyValuePair<string, string>> query)
        {
            Func<IEnumerable<TValue>, IEnumerable<TValue>> func = x => x.AsQueryable();

            foreach (var pair in query)
            {
                func = ExpandFunc(func, pair.Key, pair.Value);
            }
            return func;
        }


        private static Func<IEnumerable<TValue>, IEnumerable<TValue>> ExpandFunc<TValue>(
            Func<IEnumerable<TValue>, IEnumerable<TValue>> func, string method, string argument)
        {
            var funcs = new Dictionary<string, Func<IEnumerable<TValue>, IEnumerable<TValue>>>
            {
                {"Where", x => func(x).AsQueryable().Where(argument)},
                {"OrderBy", x => func(x).AsQueryable().OrderBy(argument)},
                {"Take", x => func(x).Take(Convert.ToInt32(argument))},
                {"Skip", x => func(x).Skip(Convert.ToInt32(argument))}
            };
            if (!funcs.ContainsKey(method))
            {
                throw new StyroQueryException(String.Format("Invalid query token: {0}",method));
            }
            return funcs[method];
        }
    }
}