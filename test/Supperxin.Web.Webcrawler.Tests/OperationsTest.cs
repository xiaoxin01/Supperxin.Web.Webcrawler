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
    }
}
