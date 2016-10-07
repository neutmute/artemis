using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ader.Text;

namespace Artemis
{
    public abstract class SearchableService<
        TSearchHeadEnum
        , TSearchQuery
        , TSearchResponse> : ISearchableService<TSearchResponse> 
            where TSearchHeadEnum : struct, IConvertible
            where TSearchQuery : ISearchQuery<TSearchHeadEnum>, new() 
    {
        
        public TSearchResponse Search(string searchText ,SearchTextFormat format = SearchTextFormat.Structured)
        {
            var tokens = ToTokens(searchText, format);
            var query = ToSearchQuery(tokens);
            return Search(query);
        }

        /// <summary>
        /// Allows consumers to intercept and transform search values - eg: enums from ints to text for neo4j 
        /// </summary>
        protected virtual void OnSearchTokenKeyValuePair(Token sourceToken, SearchKeyValuePairToken<TSearchHeadEnum> searchToken)
        {
            
        }

        protected List<SearchToken> ToTokens(string searchText, SearchTextFormat format)
        {
            var _searchTokenizerService = new SearchTokenizerService<TSearchHeadEnum>();
            _searchTokenizerService.OnSearchTokenKeyValuePair = OnSearchTokenKeyValuePair;

            if (searchText == null)
            {
                searchText = string.Empty;
            }

            // Allow searches for domain\fatuser
            searchText = searchText.Replace(@"\", @"\\");

            // strip special characters that would be interpreted in neo regex
            if (format == SearchTextFormat.Unstructured && !string.IsNullOrEmpty(searchText))
            {
                var allowedPattern = "[^-a-zA-Z0-9,_'|:!\" .\\\\]+";
                searchText = Regex.Replace(searchText, allowedPattern, "", RegexOptions.Compiled);
            }

            List<SearchToken> tokens;
            try
            {
                tokens = _searchTokenizerService.ToTokens(searchText);
            }
            catch (Exception e)
            {
                var message = $"Failed to convert '{searchText}' to tokens: " + e.Message;
                throw ArtemisException.Create(message, e);
            }


            // Add the wild cards for the user
            if (format == SearchTextFormat.Unstructured)
            {
                foreach (var token in tokens)
                {
                    var tokenAsString = token as SearchStringToken;

                    //Don't turn empty into wildcard - too many hits are useless
                    //if (token.Value == "*")
                    //{
                    //    token.Value = "";
                    //}

                    if (tokenAsString != null)
                    {
                        tokenAsString.Value = $"*{tokenAsString.Value}*";
                    }
                    var tokenAsNumber = token as SearchNumberToken;
                    if (tokenAsNumber != null)
                    {
                        tokenAsNumber.Value = $"*{tokenAsNumber.Value}*";
                    }
                }
            }
            return tokens;
        } 

        public abstract TSearchResponse Search(TSearchQuery query);

        protected TSearchQuery ToSearchQuery(List<SearchToken> tokenList)
        {
            var query = new TSearchQuery();

            foreach (var token in tokenList)
            {
                var termPredicate = TokenToSearchPredicate(token);
                query.AndPredicates.Add(termPredicate);
            }
            return query;
        }

        protected abstract SearchPredicate<TSearchHeadEnum> TokenToSearchPredicate(SearchToken token);
    }
}
