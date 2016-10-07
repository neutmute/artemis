using System;
using System.Text;

namespace Artemis
{
    public class SearchTerm<TSearchHeadEnum> : ISearchTerm where TSearchHeadEnum : struct, IConvertible
    {
        public virtual SearchMode Mode => SearchMode.Match;

        public TSearchHeadEnum Head { get; set; }

        public string Value { get; set; }

        public bool WithWildcardSuffix { get; set; }

        public bool WithWildcardPrefix { get; set; }
        
        protected Type ValueType { get; private set; }
        
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