using System;
using System.Collections.Generic;

namespace Artemis
{
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
}