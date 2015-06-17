using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Qoollo.Impl.Collector.Parser;
using StyroDB.Adapter.Converter;

namespace StyroDB.Adapter.Internal
{
    class StyroScriptParser
    {
        public static List<KeyValuePair<string, string>> CreateOrder(string script, FieldDescription idDescription,
            List<FieldDescription> userParameters)
        {
            var expr = CreateListExpr(script, idDescription);
            SetUserParams(expr, userParameters);
            AddWrapperNamesToQuery(expr);
            return expr;
        }

        private static void SetUserParams(List<KeyValuePair<string, string>> expr, List<FieldDescription> userParameters)
        {
            if(userParameters.Count==0)
                return;
            
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
        }

        private static void AddWrapperNamesToQuery(List<KeyValuePair<string, string>> expr)
        {
            for(int j=0;j<expr.Count;j++)
            {
                var tokens = expr[j].Value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
                for (int i = 0; i < tokens.Count; i++)
                {
                    var word = tokens[i].ToLower().Trim();
                    if (!word.Contains("StyroMetaData.".ToLower()) && char.IsLetter(word[0])
                        && !string.Equals(word, "and")
                        && !string.Equals(word, "false")                        
                        && !string.Equals(word, "true")
                        && !string.Equals(word, "asc")
                        && !string.Equals(word, "desc"))
                        tokens[i] = "Value." + tokens[i];
                }
                var aggr = tokens.Aggregate(string.Empty, (current, token) => current + (token + " "));
                expr[j] = new KeyValuePair<string, string>(expr[j].Key, aggr);
            }
        }

        private static List<KeyValuePair<string, string>> CreateListExpr(string script, FieldDescription idDescription)
        {
            var expr = QueryConverter.GetExpressionFromJson(script);
            string ordering = "asc";

            for (int i = 0; i < expr.Count; i++)
            {
                switch (expr[i].Key.ToLower())
                {
                    case "orderby":
                        ordering = GetOrdering(expr[i].Value);
                        expr[i] = new KeyValuePair<string, string>("OrderBy", OrderBy(expr[i].Value, idDescription));
                        break;
                    case "skip":
                        expr.RemoveAt(i--);
                        break;
                    case "take":
                        expr[i] = new KeyValuePair<string, string>("Take",
                            idDescription.PageSize.ToString(CultureInfo.InvariantCulture));
                        break;
                }
            }

            if (!expr.Exists(x => x.Key.ToLower() == "orderby"))
            {
                expr.Add(new KeyValuePair<string, string>("OrderBy", OrderByExpr(idDescription.FieldName)));
                ordering = GetOrdering(expr.Last().Value);
            }

            if (!expr.Exists(x => x.Key.ToLower() == "take"))
                expr.Add(new KeyValuePair<string, string>("Take",
                    idDescription.PageSize.ToString(CultureInfo.InvariantCulture)));

            if (!idDescription.IsFirstAsk)
                expr.Add(Skip(idDescription, ordering));
            return expr;
        }

        #region Create List

        private static string OrderBy(string value, FieldDescription idDescription)
        {
            string ordering = GetOrdering(value);
            return OrderByExpr(idDescription.FieldName, ordering);
        }

        private static string GetOrdering(string value)
        {
            string ordering = "asc";
            if (value.Contains("desc"))
                ordering = "desc";
            return ordering;
        }

        private static string OrderByExpr(string fieldName, string orderType = "asc")
        {
            return fieldName + " " + orderType;
        }

        private static KeyValuePair<string, string> Skip(FieldDescription idDescription, string ordering)
        {
            string operation = ordering == "asc" ? ">" : "<";
            return new KeyValuePair<string, string>("Where", idDescription.FieldName + operation + idDescription.Value);
        }

        #endregion
    }
}
