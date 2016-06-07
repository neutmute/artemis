using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artemis;
using Neo4jClient.Cypher;

namespace Artemis.Neo4j
{
    public static class ArtemisData
    {
        public static ICypherFluentQuery AppendWhereClause<TSearchHeadEnum>(
            ICypherFluentQuery q
            , List<SearchPredicate<TSearchHeadEnum>> predicates
            , Func<SearchTerm<TSearchHeadEnum>, string> termToWhereText
            , bool whereAdded = false)
             where TSearchHeadEnum : struct, IConvertible
        {
            var firstWhereAdded = whereAdded;
            foreach (var predicate in predicates)
            {
                var whereText = PredicateToWhereText(predicate, termToWhereText);

                if (!firstWhereAdded)
                {
                    q = q.Where(whereText);
                    firstWhereAdded = true;
                }
                else
                {
                    q = q.AndWhere(whereText);
                }
            }

            return q;
        }

        private static string PredicateToWhereText<TSearchHeadEnum>(
            SearchPredicate<TSearchHeadEnum> predicate
            ,Func<SearchTerm<TSearchHeadEnum>, string> termToWhereText) where TSearchHeadEnum : struct, IConvertible
        {
            var whereText = termToWhereText(predicate.Term);

            if (predicate.HasOrPredicate)
            {
                var orText = PredicateToWhereText(predicate.OrPredicate, termToWhereText); //yes resharper, it is recursive
                whereText = AsLogicalOr(whereText, orText);
            }
            return whereText;
        }

        static string AsLogicalOr(params string[] predicates)
        {   
            return "(" + string.Join(" OR ", predicates) + ")";
        }
    }
}
