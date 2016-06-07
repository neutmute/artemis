using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Artemis;
using Neo4jClient.Cypher;

namespace Artemis.Neo4j
{
    public static class WhereBuilder
    {
        public static string ToText<TSearchHeadEnum, M, P>(Expression<Func<M, P>> expression, SearchTerm<TSearchHeadEnum> term) where TSearchHeadEnum : struct, IConvertible
        {
            var me = expression.Body as MemberExpression;
            var alias = me.Expression.ToString();
            var property = CypherFluentQuery.ApplyCamelCase(true, me.Member.Name);

            var whereText = $"{alias}.{property} {term.ToPredicateConditionAndBody()}";
            return whereText;
        }

        public static string ToPredicateConditionAndBody<TSearchHeadEnum>(this SearchTerm<TSearchHeadEnum> term) where TSearchHeadEnum : struct, IConvertible
        {
            var termBuilderMap = new Dictionary<SearchMode, ISearchTermBuilder<TSearchHeadEnum>>();

            termBuilderMap.Add(SearchMode.Match, new SearchTermBuilder<TSearchHeadEnum>());
            termBuilderMap.Add(SearchMode.AntiMatch, new AntiSearchTermBuilder<TSearchHeadEnum>());

            return termBuilderMap[term.Mode].ToPredicateConditionAndBody(term);
        }

        private interface ISearchTermBuilder<TSearchHeadEnum> where TSearchHeadEnum : struct, IConvertible
        {
            string ToPredicateConditionAndBody(SearchTerm<TSearchHeadEnum> term);
        }

        private class AntiSearchTermBuilder<TSearchHeadEnum> : BaseSearchTermBuilder, ISearchTermBuilder<TSearchHeadEnum>  where TSearchHeadEnum : struct, IConvertible
        {
            public string ToPredicateConditionAndBody(SearchTerm<TSearchHeadEnum> term)
            {

                var sb = new StringBuilder();

                switch (term.TypeCode)
                {
                    case TypeCode.String:
                        sb.Append("=~ ");
                        sb.Append("'(?i)");
                        sb.Append($"^((?!{GetRegexEscapedTermValue(term)}).)*$");
                        sb.Append("'");
                        break;

                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Int16:
                        sb.Append("!= ");
                        sb.Append(term.Value);
                        break;

                    default:
                        throw ArtemisException.Create($"ToPredicateConditionAndBody needs to be taught how to handle {term.TypeCode}");
                }

                return sb.ToString();
            }
        }

        private class BaseSearchTermBuilder
        {
            protected string GetRegexEscapedTermValue(ISearchTerm term)
            {
                return term.Value.Replace("'", "\\'");
            }
        }

        private class SearchTermBuilder<TSearchHeadEnum> :  BaseSearchTermBuilder, ISearchTermBuilder<TSearchHeadEnum> where TSearchHeadEnum : struct, IConvertible
        {
            public string ToPredicateConditionAndBody(SearchTerm<TSearchHeadEnum> term)
            {

                var sb = new StringBuilder();

                switch (term.TypeCode)
                {
                    case TypeCode.String:
                        sb.Append("=~ ");
                        sb.Append("'(?i)");
                        sb.Append(term.WithWildcardPrefix ? ".*" : string.Empty);
                        sb.Append(GetRegexEscapedTermValue(term));
                        sb.Append(term.WithWildcardSuffix ? ".*" : string.Empty);
                        sb.Append("'");
                        break;
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Int16:
                        sb.Append("= ");
                        sb.Append(term.Value);
                        break;
                    default:
                        throw ArtemisException.Create($"ToPredicateConditionAndBody needs to be taught how to handle {term.TypeCode}");
                }

                return sb.ToString();
            }
        }
    }
}