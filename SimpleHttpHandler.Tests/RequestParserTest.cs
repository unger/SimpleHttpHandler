using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpHandler.Test
{
    using System.Collections.Specialized;
    using System.Web;

    using Moq;

    using NUnit.Framework;

    using Newtonsoft.Json.Linq;

    using SimpleHttpHandler.RequestHelpers;
    using SimpleHttpHandler.Test.Fakes;

    [TestFixture]
    public class RequestParserTest
    {
        private RequestParser requestParser;

        [SetUp]
        public void SetUp()
        {
            this.requestParser = new RequestParser(new ParamSerializer());
        }

        [Test]
        public void Parse_ParamQueryStringOneParameter()
        {
            // Arrange
            var fakeRequest = new FakeRawHttpRequest("a=1", string.Empty);

            // Act
            var data = this.requestParser.GetData(fakeRequest);

            // Assert
            Assert.AreEqual(new JObject { { "a", "1" } }, data);
        }

        [Test]
        public void Parse_ParamQueryStringTwoParameter()
        {
            // Arrange
            var fakeRequest = new FakeRawHttpRequest("a=1&b=2", string.Empty);

            // Act
            var data = this.requestParser.GetData(fakeRequest);

            // Assert
            Assert.AreEqual(new JObject { { "a", "1" }, { "b", "2" } }, data);
        }

        [Test]
        public void Parse_JsonQueryStringOneParameter()
        {
            // Arrange
            var fakeRequest = new FakeRawHttpRequest("{\"a\":\"1\"}", string.Empty);

            // Act
            var data = this.requestParser.GetData(fakeRequest);

            // Assert
            Assert.AreEqual(new JObject { { "a", "1" } }, data);
        }

        [Test]
        public void Parse_JsonQueryStringArrayParameter()
        {
            // Arrange
            var fakeRequest = new FakeRawHttpRequest("[{\"a\":\"1\"},{\"b\":\"2\"}]", string.Empty);

            // Act
            var data = this.requestParser.GetData(fakeRequest);

            // Assert
            Assert.AreEqual(1, data.Properties().Count());
            Assert.AreEqual(
                new JArray { new JObject { { "a", "1" } }, new JObject { { "b", "2" } } },
                data[data.Properties().First().Name]);
        }


        [Test]
        public void Merge_JsonQueryWithJsonFormParameter()
        {
            // Arrange
            var fakeRequest = new FakeRawHttpRequest("{\"a\":\"1\"}", "{\"b\":\"2\"}");

            // Act
            var data = this.requestParser.GetData(fakeRequest);

            // Assert
            Assert.AreEqual(new JObject { { "a", "1" }, { "b", "2" } }, data);
        }

        [Test]
        public void Merge_JsonQueryWithJsonFormOverwriteProperties()
        {
            // Arrange
            var fakeRequest = new FakeRawHttpRequest("{\"a\":\"1\",\"b\":\"1\"}", "{\"b\":\"2\"}");

            // Act
            var data = this.requestParser.GetData(fakeRequest);

            // Assert
            Assert.AreEqual(new JObject { { "a", "1" }, { "b", "2" } }, data);
        }

        [Test]
        public void Merge_ParamQueryStringWithParamForm()
        {
            // Arrange
            var fakeRequest = new FakeRawHttpRequest("a=1&b=2", "c=3&d=4");

            // Act
            var data = this.requestParser.GetData(fakeRequest);

            // Assert
            Assert.AreEqual(new JObject { { "a", "1" }, { "b", "2" }, { "c", "3" }, { "d", "4" } }, data);
        }


        [Test]
        public void Merge_ParamQueryStringWithParamFormOverwrite()
        {
            // Arrange
            var fakeRequest = new FakeRawHttpRequest("a=1&b=2", "a=3&b=4");

            // Act
            var data = this.requestParser.GetData(fakeRequest);

            // Assert
            Assert.AreEqual(new JObject { { "a", "3" }, { "b", "4" } }, data);
        }

    }
}
