using System;
using System.Collections.Generic;

namespace Artemis
{
    public interface ISearchQuery<TSearchHeadEnum> where TSearchHeadEnum : struct, IConvertible
    {
        List<SearchPredicate<TSearchHeadEnum>> AndPredicates { get; set; }
        bool HasPredicates { get; }

        /// <summary>
        /// So it can be rendered to the client
        /// </summary>
        string DisplayText { get; }
    }
}