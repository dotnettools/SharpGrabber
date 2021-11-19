using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using Jint;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SharpGrabber.BlackWidow.Tests.Interpreter.JavaScript
{
    public class JintMultiTypeConverterTests
    {
        private delegate int SumDelegate(int a, int b);
        private delegate string StringSumDelegate(string a, string b);

        [Fact]
        public void Test_MethodConversionWithInputAndOutputConversion()
        {
            var engine = new Engine();
            var converter = JintMultiTypeConverter.CreateDefault(engine);
            SumDelegate Sum = (a, b) => a + b;
            var stringSum = (StringSumDelegate)converter.Convert(Sum, typeof(StringSumDelegate), null);
            var result = stringSum("10", "20");
            Assert.Equal("30", result);
        }
    }
}
