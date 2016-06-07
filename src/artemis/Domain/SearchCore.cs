using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public interface ISearchQuery<TSearchHeadEnum> where TSearchHeadEnum : struct, IConvertible
    {
        List<SearchPredicate<TSearchHeadEnum>> AndPredicates { get; set; }
        bool HasPredicates { get; }

        /// <summary>
        /// So it can be rendered to the client
        /// </summary>
        string DisplayText { get; }
    }

    public class SearchQuery<TSearchHeadEnum> : ISearchQuery<TSearchHeadEnum> where TSearchHeadEnum : struct, IConvertible
    {
        public List<SearchPredicate<TSearchHeadEnum>> AndPredicates { get; set; }
        public bool HasPredicates => AndPredicates.Count > 0;

        public SearchQuery()
        {
            AndPredicates = new List<SearchPredicate<TSearchHeadEnum>>();
        }

        /// <summary>
        /// So it can be rendered to the client
        /// </summary>
        public string DisplayText => AndPredicates.ToStringX(" AND ");

        public override string ToString()
        {
            return $"Predicates={AndPredicates.ToStringX(" AND ")}";
        }
    }

    public class SearchResponse<TSearchHeadEnum, TResult> where TSearchHeadEnum : struct, IConvertible
    {
        public string DebugText { get; set; }

        public SearchQuery<TSearchHeadEnum> Query { get; set; }

        public List<TResult> Results { get; set; }

        public SearchResponse()
        {
            Results = new List<TResult>();
        }

        public override string ToString()
        {
            var searchType = typeof(TSearchHeadEnum).Name.Replace("SearchHead", "");
            return $"Search={searchType}: Query={Query}, Results.Count={Results.Count}";
        }
    }

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

    public class SearchAntiTerm<TSearchHeadEnum> : SearchTerm<TSearchHeadEnum> where TSearchHeadEnum : struct, IConvertible
    {
        public override SearchMode Mode => SearchMode.AntiMatch;
    }

    public interface ISearchTerm
    {
        string Value { get; set; }
    }

    public enum SearchMode
    {
        Match,
        AntiMatch
    }

    public class SearchTerm<TSearchHeadEnum> : ISearchTerm where TSearchHeadEnum : struct, IConvertible
    {
        public virtual SearchMode Mode => SearchMode.Match;

        public TSearchHeadEnum Head { get; set; }

        public string Value { get; set; }

        public bool WithWildcardSuffix { get; set; }

        public bool WithWildcardPrefix { get; set; }

//        [JsonIgnore]
        protected Type ValueType { get; private set; }

        //[JsonIgnore]
        public TypeCode TypeCode => Type.GetTypeCode(ValueType);

        protected SearchTerm()
        {
            
        }

        public static SearchTerm<TSearchHeadEnum> CreateAnti(TSearchHeadEnum head, string value, Type type = null, Func<SearchTerm<TSearchHeadEnum>> searchTermFactory = null)
        {
            return Create(() => new SearchAntiTerm<TSearchHeadEnum>(), head, value, type);
        }

        public static SearchTerm<TSearchHeadEnum> Create(TSearchHeadEnum head, string value, Type type = null)
        {
            return Create(() => new SearchTerm<TSearchHeadEnum>(), head, value, type);
        }

        public static SearchTerm<TSearchHeadEnum> Create<TValue>(TSearchHeadEnum head, TValue value)
        {
            return Create(head, value.ToString(), typeof(TValue));
        }

        public static SearchTerm<TSearchHeadEnum> CreateAnti<TValue>(TSearchHeadEnum head, TValue value)
        {
            return CreateAnti(head, value.ToString(), typeof(TValue));
        }

        private static SearchTerm<TSearchHeadEnum> Create(Func<SearchTerm<TSearchHeadEnum>> searchTermFactory, TSearchHeadEnum head, string value, Type type = null)
        {
            SearchTerm<TSearchHeadEnum> p;

            if (searchTermFactory == null)
            {
                p = new SearchTerm<TSearchHeadEnum>();
            }
            else
            {
                p = searchTermFactory();
            }

            if (type == null)
            {
                type = typeof(string);
            }

            p.Head = head;
            p.Value = value;
            p.ValueType = type;

            if (p.Value.EndsWith("*"))
            {
                p.Value = p.Value.Substring(0, value.Length - 1);
                p.WithWildcardSuffix = true;
            }

            if (p.Value.StartsWith("*"))
            {
                p.Value = p.Value.Substring(1);
                p.WithWildcardPrefix = true;
            }

            return p;
        }

        
        public override string ToString()
        {
            if (WithWildcardSuffix || WithWildcardPrefix || Mode == SearchMode.AntiMatch)
            {
                var sb = new StringBuilder();
                var adjective = Mode == SearchMode.Match ? "LIKE" : "NOT LIKE";
                sb.Append($"{Head} {adjective} '");

                if (WithWildcardPrefix)
                {
                    sb.Append("*");
                }

                sb.Append(Value);

                if (WithWildcardSuffix)
                {
                    sb.Append("*");
                }

                sb.Append("'");
                return sb.ToString();
            }

            if (string.IsNullOrEmpty(Value))
            {
                // handles attribute case - eg: !HasError
                return Head.ToString();
            }

            if (TypeCode == TypeCode.String)
            {
                return $"{Head}='{Value}'";
            }

            return $"{Head}={Value}";
        }
    }
}
