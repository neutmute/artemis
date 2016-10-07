using System;
using System.Text;

namespace Artemis
{
    public class SearchPredicate<TSearchHeadEnum> where TSearchHeadEnum : struct, IConvertible
    {
        public SearchTerm<TSearchHeadEnum> Term { get; set; }

        public bool HasTerm => Term != null;

        public SearchPredicate<TSearchHeadEnum> OrPredicate { get; set; }

        public SearchPredicate<TSearchHeadEnum> AndPredicate { get; set; }

        public bool HasOrPredicate => OrPredicate != null;

        public bool HasAndPredicate => AndPredicate != null;

        public static SearchPredicate<TSearchHeadEnum> Create<TValue>(TSearchHeadEnum head, TValue value, Type type = null)
        {
            var p = new SearchPredicate<TSearchHeadEnum>();
            p.Term = SearchTerm<TSearchHeadEnum>.Create(head, value.ToString(), type);
            return p;
        }

        public static SearchPredicate<TSearchHeadEnum> CreateAnti<TValue>(TSearchHeadEnum head, TValue value)
        {
            var p = new SearchPredicate<TSearchHeadEnum>();
            p.Term = SearchTerm<TSearchHeadEnum>.CreateAnti(head, value);
            return p;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");

            sb.Append(Term);

            if (HasOrPredicate)
            {
                sb.Append(" OR " + OrPredicate);
            }

            if (HasAndPredicate)
            {
                sb.Append(" AND " + AndPredicate);
            }

            sb.Append(")");

            return sb.ToString();
        }
    }
}