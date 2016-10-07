using System;
using System.Collections.Generic;

namespace Artemis
{
    public static class SearchParameterExtensions
    {
        public static void Add<TSearchHeadEnum, TValue>(this List<SearchTerm<TSearchHeadEnum>> target, TSearchHeadEnum head, TValue value) where TSearchHeadEnum : struct, IConvertible
        {
            var q = SearchTerm<TSearchHeadEnum>.Create(head, value);
            target.Add(q);
        }

        public static void Add<TSearchHeadEnum, TValue>(this List<SearchPredicate<TSearchHeadEnum>> target, TSearchHeadEnum head, TValue value, bool withWildcardSuffix = false) where TSearchHeadEnum : struct, IConvertible
        {
            var p = SearchPredicate<TSearchHeadEnum>.Create(head, value);
            p.Term.WithWildcardSuffix = withWildcardSuffix;
            target.Add(p);
        }

        public static string ToStringX<TSearchHeadEnum>(this List<SearchQuery<TSearchHeadEnum>> target, string delimiter = ",") where TSearchHeadEnum : struct, IConvertible
        {
            return string.Join(delimiter, target);
        }

        public static string ToStringX<TSearchHeadEnum>(this List<SearchPredicate<TSearchHeadEnum>> target, string delimiter = ", ") where TSearchHeadEnum : struct, IConvertible
        {
            return string.Join(delimiter, target);
        }

        public static string ToStringX<TSearchHeadEnum>(this IEnumerable<TSearchHeadEnum> target, string delimiter = ",") where TSearchHeadEnum : struct, IConvertible
        {
            return string.Join(delimiter, target);
        }
    }
}