using Supperxin.Web.Webcrawler.Operations;
using System;
using Xunit;

namespace Supperxin.Web.Webcrawler.Tests
{
    public class OperationsTest
    {
        [Fact]
        public void TestOpRegexReplace()
        {
            //Given
            IOperation operation = new OpRegexReplace();

            //When
            var result = operation.Operate("https://www.v2ex.com/t/537750#reply1", "#reply\\d+", "") as string;

            //Then
            Assert.Equal("https://www.v2ex.com/t/537750", result);
        }

        [Fact]
        public void TestOperationFactoryCache()
        {
            //Given
            IOperationFactory factory = new OperationFactory();

            //When
            var operation1 = factory.MakeOperation(typeof(OpRegexReplace).Name);
            var operation2 = factory.MakeOperation(typeof(OpRegexReplace).Name);
            var operation3 = new OpRegexReplace();

            //Then
            Assert.Same(operation1, operation2);
            Assert.NotSame(operation1, operation3);
        }
    }
}
