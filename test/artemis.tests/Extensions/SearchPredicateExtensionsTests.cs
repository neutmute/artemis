using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Artemis.Tests.Extensions
{
    public class SearchPredicateExtensionsTests
    {

        [Fact]
        public void FindHead()
        {
            var predicate1 = SearchPredicate<EntitySearchHead>.Create(EntitySearchHead.Name, "archer");
            predicate1.OrPredicate = SearchPredicate<EntitySearchHead>.Create(EntitySearchHead.State, "intoxicated");
            
            var result = SearchPredicateExtensions.FindHead(predicate1, EntitySearchHead.State);
            Assert.Equal(1, result.Count);
            Assert.Equal(EntitySearchHead.State, result[0].Term.Head);
        }
    }
}
