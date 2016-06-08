using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ader.Text;

namespace Artemis
{
    /// <summary>
    /// How sophisticated is the query contained
    /// </summary>
    public enum SearchTextFormat
    {
        Unspecified,

        /// <summary>
        /// User typed random crap
        /// </summary>
        Unstructured,

        /// <summary>
        /// well formed query
        /// </summary>
        Structured
    }

    public interface ISearchTokenizerService
    {
        List<SearchToken> ToTokens(string searchText);
    }


    public class SearchTokenizerService<TSearchKeyOperator> : ISearchTokenizerService 
        where TSearchKeyOperator : struct, IConvertible
    {
        public const string KeyValueArrayDelimiter = "|";

        public Action<Token, SearchKeyValuePairToken<TSearchKeyOperator>> OnSearchTokenKeyValuePair { get; set; }

        public List<SearchToken> ToTokens(string searchText)
        {
            Token token;
            var tokenizer = new StringTokenizer(searchText);
            var tokenList = new List<SearchToken>();

            do
            {
                token = tokenizer.Next();

                switch (token.Kind)
                {
                    case TokenKind.NotOperator:
                        token = tokenizer.Next();
                        tokenList.Add(new AntiStringToken { Value = token.Value });
                        break;

                    case TokenKind.AttributeOperator:
                        var attributeValue = token.Value.Substring(1);
                        tokenList.Add(new SearchAttributeToken { Value = attributeValue });
                        break;

                    case TokenKind.KeyValueOperator:
                        Token operand = tokenizer.Next();
                        var keyAsString = token.Value.Remove(token.Value.Length-1).ToLower(); // strip off the trailing colon (:)

                        TSearchKeyOperator keyAsEnum;
                        
                        if (!Enum.TryParse(keyAsString, true, out keyAsEnum))
                        {
                            var allowedKeys = ((TSearchKeyOperator[]) Enum.GetValues(typeof(TSearchKeyOperator))).ToList();
                            allowedKeys.RemoveAll(m => Convert.ToInt32(m) == 0);
                            var allowedKeyCsv = string.Join(", ", allowedKeys);
                            throw ArtemisException.Create($"{keyAsString} is not a recognised search key. Valid values are: {allowedKeyCsv}");
                        }
                        
                        var keyValueTokenPair = new SearchKeyValuePairToken<TSearchKeyOperator> { Key = keyAsEnum, Value = operand.Value };

                        // Allow transformation of search input values
                        if (OnSearchTokenKeyValuePair != null)
                        {
                            OnSearchTokenKeyValuePair(operand, keyValueTokenPair);
                        }

                        tokenList.Add(keyValueTokenPair);
                        break;

                    case TokenKind.Number:
                        tokenList.Add(new SearchNumberToken{ Value = token.Value });
                        break;

                    case TokenKind.Word:
                    case TokenKind.QuotedString:
                        tokenList.Add(new SearchStringToken{ Value = token.Value });
                        break;
                }
            }
            while (token.Kind != TokenKind.EOF);
            
            return tokenList;
        }
    }
}
