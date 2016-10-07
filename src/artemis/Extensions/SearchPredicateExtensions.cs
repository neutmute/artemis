using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artemis
{
    public static class SearchPredicateExtensions
    {
        /// <summary>
        /// Recursively find the search head we are looking for
        /// </summary>
        public static List<SearchPredicate<TSearchHeadEnum>> FindHead<TSearchHeadEnum>(
            List<SearchPredicate<TSearchHeadEnum>> target
            ,TSearchHeadEnum head)
            where TSearchHeadEnum : struct, IConvertible
        {
            var output = new List<SearchPredicate<TSearchHeadEnum>>();
            foreach (var predicate in target)
            {
                output.AddRange(FindHead(predicate, head));
            }
            return output;
        }

        public static List<SearchPredicate<TSearchHeadEnum>> FindHead<TSearchHeadEnum>(
           SearchPredicate<TSearchHeadEnum> target
           , TSearchHeadEnum head)
           where TSearchHeadEnum : struct, IConvertible
        {
            var output = new List<SearchPredicate<TSearchHeadEnum>>();

            if (target.HasTerm && target.Term.Head.Equals(head))
            {
                output.Add(target);
            }

            if (target.HasAndPredicate)
            {
                output.AddRange(FindHead(target.AndPredicate,head));
            }

            if (target.HasOrPredicate)
            {
                output.AddRange(FindHead(target.OrPredicate, head));
            }

            return output;
        }
    }
}
