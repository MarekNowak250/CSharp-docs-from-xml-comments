using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DescriptionGenerator.Core.Tests
{
    public class MethodContainerTests
    {
        [Fact]
        public void MethodContainerGeneratesValidInputJson()
        {
            var sut = new MethodContainer("m1", "t1", "xd", new List<(string, Type)>() 
            { 
                ("name", typeof(string)), 
                ("count", typeof(int)), 
                ("testClass",typeof(TestClass)) 
            }, typeof(TestClass2));

            var input = sut.GetInputJson();
            "{\"name\": null,\r\n\"count\": 0,\r\n\"testClass\": null}".Should().Be(input);      
        }

        [Fact]
        public void MethodContainerGeneratesValidOutputJsonForObject()
        {
            var sut = new MethodContainer("m1", "t1", "xd", new List<(string, Type)>()
            {
                ("name", typeof(string)),
                ("count", typeof(int)),
                ("testClass",typeof(TestClass))
            }, typeof(TestClass2));

            var output = sut.GetOutputJson();
            "{\"Name\":null,\"Value\":0,\"IsTrue\":false}".Should().Be(output);
        }

        [Theory]
        [InlineData(typeof(string), "")]
        [InlineData(typeof(int), "0")]
        [InlineData(typeof(bool), "false")]
        public void MethodContainerGeneratesValidOutputJsonForVaueType( Type output, string jsonData )
        {
            var sut = new MethodContainer("m1", "t1", "xd", new List<(string, Type)>()
            {
                ("name", typeof(string)),
                ("count", typeof(int)),
                ("testClass",typeof(TestClass))
            }, output);

            var outputJson = sut.GetOutputJson();
            jsonData.Should().Be(outputJson);
        }

        internal class TestClass
        {
            public string Name { get; set; }
            public int Value { get; set; }

            public TestClass2 nested { get; set; }
        }

        internal class TestClass2
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public bool IsTrue { get; set; }
        }
    }
}
