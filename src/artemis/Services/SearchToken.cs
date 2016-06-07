using System;

namespace Artemis
{
    public enum SearchTokenType
    {
        String,
        Number,
        KeyValuePair,
        AntiWord,
        Attribute
    }

    public abstract class SearchToken
    {
        public abstract SearchTokenType Type { get;  }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"Type={Type}, Value='{Value}'";
        }
    }

    public class SearchStringToken : SearchToken
    {
        public override SearchTokenType Type => SearchTokenType.String;
    }

    public class AntiStringToken : SearchToken
    {
        public override SearchTokenType Type => SearchTokenType.AntiWord;
    }

    public class SearchNumberToken : SearchToken
    {
        public override SearchTokenType Type => SearchTokenType.Number;
    }

    public class SearchAttributeToken : SearchToken
    {
        public override SearchTokenType Type => SearchTokenType.Attribute;
    }

    public class SearchKeyValuePairToken<TSearchKeyOperator> : SearchToken where TSearchKeyOperator : struct, IConvertible
    {
        public override SearchTokenType Type => SearchTokenType.KeyValuePair;

        public TSearchKeyOperator Key { get; set; }

        public override string ToString()
        {
            return $"Key={Key}, {base.ToString()}";
        }
    }
}