using System;
using System.Collections.Generic;
using System.Linq;
using Qoollo.Impl.Collector.Parser;
using StyroDB.Adapter.Converter;

namespace StyroDB.Adapter.Internal
{
    class StyroScriptParser
    {
        public static void CreateOrder(string script, FieldDescription idDescription,
            List<FieldDescription> userParameters)
        {
            var expr = CreateListExpr(script, idDescription);
            SetUserParams(expr, userParameters);
        }

        private static void SetUserParams(List<KeyValuePair<string, string>> expr, List<FieldDescription> userParameters)
        {
            for (int i = 0; i < expr.Count; i++)
            {
                foreach (var parameter in userParameters)
                {
                    if (expr[i].Value.Contains("@" + parameter.FieldName))
                    {
                        expr[i] = new KeyValuePair<string, string>(expr[i].Key,
                            expr[i].Value.Replace("@" + parameter.FieldName, parameter.Value.ToString()));
                    }                    
                }
            }
            //TODO replace field -> Value.field
            //except StyroMetaData.IsDelete
        }

        #region Create List

        private static List<KeyValuePair<string, string>> CreateListExpr(string script, FieldDescription idDescription)
        {
            var expr = QueryConverter.GetExpressionFromJson(script);
            string ordering = "asc";

            for (int i = 0; i < expr.Count; i++)
            {
                switch (expr[i].Key.ToLower())
                {
                    case "take":
                        ordering = GetOrdering(expr[i].Value);
                        expr[i] = new KeyValuePair<string, string>("take", Take(expr[i].Value, idDescription));
                        break;
                    case "skip":
                        expr.RemoveAt(i--);
                        break;
                }
            }

            if (!expr.Exists(x => x.Key.ToLower() == "take"))
            {
                expr.Add(new KeyValuePair<string, string>("take", TakeExpr(idDescription.FieldName)));
                ordering = GetOrdering(expr.Last().Value);
            }

            if (!idDescription.IsFirstAsk)
                expr.Add(Skip(idDescription, ordering));
            return expr;
        }

        private static string Take(string value, FieldDescription idDescription)
        {
            string ordering = "asc";
            if (value.Contains("desc"))
                ordering = "desc";
            return TakeExpr(idDescription.FieldName, ordering);
        }

        private static string GetOrdering(string value)
        {
            string ordering = "asc";
            if (value.Contains("desc"))
                ordering = "desc";
            return ordering;
        }

        private static string TakeExpr(string fieldName, string orderType = "asc")
        {
            return fieldName + " " + orderType;
        }

        private static KeyValuePair<string, string> Skip(FieldDescription idDescription, string ordering)
        {
            string operation = ordering == "asc" ? ">" : "<";
            return new KeyValuePair<string, string>("where", idDescription.FieldName + operation + idDescription.Value);
        }

        #endregion
    }
}
