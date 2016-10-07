using System;
using System.Collections.Generic;

namespace Artemis
{
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
}