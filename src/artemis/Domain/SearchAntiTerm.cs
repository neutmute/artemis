using System;

namespace Artemis
{
    public class SearchAntiTerm<TSearchHeadEnum> : SearchTerm<TSearchHeadEnum> where TSearchHeadEnum : struct, IConvertible
    {
        public override SearchMode Mode => SearchMode.AntiMatch;
    }
}