using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artemis;
using Xunit;

namespace Artemis.Tests
{
    public class SearchTokenizerServiceTests
    {
        public SearchTokenizerService<EntitySearchHead> GetSystemUnderTest()
        {
            return new SearchTokenizerService<EntitySearchHead>();
        }

        [Fact]
        public void KeyValueArray()
        {
            var sut = GetSystemUnderTest();

            var tokens = sut.ToTokens("state:one|two|three");

            // CodeGen.GenerateAssertions(tokens, "tokens"); // The following assertions were generated on 14-Aug-2015
            #region CodeGen Assertions
            Assert.Equal(1, tokens.Count);
            Assert.Equal(SearchTokenType.KeyValuePair, ((SearchKeyValuePairToken<EntitySearchHead>)tokens[0]).Type);
            Assert.Equal(EntitySearchHead.State, ((SearchKeyValuePairToken<EntitySearchHead>)tokens[0]).Key);
            Assert.Equal("one|two|three", ((SearchKeyValuePairToken<EntitySearchHead>)tokens[0]).Value);
            #endregion
        }

        [Fact]
        public void MultipleWords()
        {
            var sut = GetSystemUnderTest();

            var tokens = sut.ToTokens("one two three");

            // CodeGen.GenerateAssertions(tokens, "tokens"); // The following assertions were generated on 14-Aug-2015
            #region CodeGen Assertions
            Assert.Equal(3, tokens.Count);
            Assert.Equal(SearchTokenType.String, tokens[0].Type);
            Assert.Equal("one", tokens[0].Value);
            Assert.Equal(SearchTokenType.String, tokens[1].Type);
            Assert.Equal("two", tokens[1].Value);
            Assert.Equal(SearchTokenType.String, tokens[2].Type);
            Assert.Equal("three", tokens[2].Value);
            #endregion
        }


        [Fact]
        public void Operators()
        {
            var sut = GetSystemUnderTest();

            var tokens = sut.ToTokens("state:nsw two three");

            // CodeGen.GenerateAssertions(tokens, "tokens"); // The following assertions were generated on 14-Aug-2015
            #region CodeGen Assertions
            Assert.Equal(3, tokens.Count);
            Assert.Equal(SearchTokenType.KeyValuePair, ((SearchKeyValuePairToken<EntitySearchHead>)tokens[0]).Type);
            Assert.Equal(EntitySearchHead.State, ((SearchKeyValuePairToken<EntitySearchHead>)tokens[0]).Key);
            Assert.Equal("nsw", ((SearchKeyValuePairToken<EntitySearchHead>)tokens[0]).Value);
            Assert.Equal(SearchTokenType.String, tokens[1].Type);
            Assert.Equal("two", tokens[1].Value);
            Assert.Equal(SearchTokenType.String, tokens[2].Type);
            Assert.Equal("three", tokens[2].Value);
            #endregion
        }

        [Fact]
        public void QuotedString()
        {
            var sut = GetSystemUnderTest();

            var tokens = sut.ToTokens("\"that is how you get ants\" mallory");

            // CodeGen.GenerateAssertions(tokens, "tokens"); // The following assertions were generated on 14-Aug-2015
            #region CodeGen Assertions
            Assert.Equal(2, tokens.Count);
            Assert.Equal(SearchTokenType.String, tokens[0].Type);
            Assert.Equal("that is how you get ants", tokens[0].Value);
            Assert.Equal(SearchTokenType.String, tokens[1].Type);
            Assert.Equal("mallory", tokens[1].Value);
            #endregion
        }

        [Fact]
        public void WildcardString()
        {
            var sut = GetSystemUnderTest();

            var tokens = sut.ToTokens("\"that is how you get ants*\" mallory*");

            // CodeGen.GenerateAssertions(tokens, "tokens"); // The following assertions were generated on 14-Aug-2015
            #region CodeGen Assertions
            Assert.Equal(2, tokens.Count);
            Assert.Equal(SearchTokenType.String, tokens[0].Type);
            Assert.Equal("that is how you get ants*", tokens[0].Value);
            Assert.Equal(SearchTokenType.String, tokens[1].Type);
            Assert.Equal("mallory*", tokens[1].Value);
            #endregion
        }

        [Fact]
        public void Numbers()
        {
            var sut = GetSystemUnderTest();

            var tokens = sut.ToTokens("123 456");

            // CodeGen.GenerateAssertions(tokens, "tokens"); // The following assertions were generated on 14-Aug-2015
            #region CodeGen Assertions
            Assert.Equal(2, tokens.Count);
            Assert.Equal(SearchTokenType.Number, tokens[0].Type);
            Assert.Equal("123", tokens[0].Value);
            Assert.Equal(SearchTokenType.Number, tokens[1].Type);
            Assert.Equal("456", tokens[1].Value);
            #endregion
        }
    }
}
