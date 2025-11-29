using System.Text.Json;
using System.Xml.Serialization;
using Xunit;
using Shouldly;

namespace BlazorRest.Tests
{
    public class ResponseParserTests
    {
        public class SampleModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        [Fact]
        public void Parse_ValidJson_ReturnsObject()
        {
            const string json = "{\"Id\":1,\"Name\":\"Test\"}";

            var result = ResponseParser.Parse<SampleModel>(json, "application/json");

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(1);
            result.Name.ShouldBe("Test");
        }

        [Fact]
        public void Parse_InvalidJson_ReturnsDefault()
        {
            const string invalidJson = "this is not json";

            var result = ResponseParser.Parse<SampleModel>(invalidJson, "application/json");

            result.ShouldBeNull();
        }

        [Fact]
        public void Parse_EmptyContent_ReturnsDefault()
        {
            var result = ResponseParser.Parse<SampleModel>("", "application/json");
            result.ShouldBeNull();
        }

        [Fact]
        public void Parse_ValidXml_ReturnsObject()
        {
            var serializer = new XmlSerializer(typeof(SampleModel));
            var xml = "";
            using (var sw = new StringWriter())
            {
                serializer.Serialize(sw, new SampleModel { Id = 5, Name = "XMLTest" });
                xml = sw.ToString();
            }
            
            var result = ResponseParser.Parse<SampleModel>(xml, "application/xml");

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(5);
            result.Name.ShouldBe("XMLTest");
        }

        [Fact]
        public void Parse_InvalidXml_ReturnsDefault()
        {
            const string invalidXml = "<SampleModel><Id>5</Id><Name>XMLTest</Name>";

            var result = ResponseParser.Parse<SampleModel>(invalidXml, "application/xml");

            result.ShouldBeNull();
        }

        [Fact]
        public void Parse_StringType_ReturnsContent()
        {
            const string content = "Hello World";

            var result = ResponseParser.Parse<string>(content);

            result.ShouldBe("Hello World");
        }

        [Theory]
        [InlineData("123", 123)]
        [InlineData("true", true)]
        public void Parse_ValueTypes_ReturnsCorrectValue<T>(string content, T expected)
        {
            var result = ResponseParser.Parse<T>(content);

            result.ShouldBe(expected);
        }

        [Fact]
        public void Parse_NullContent_ReturnsDefault()
        {
            var result = ResponseParser.Parse<SampleModel>(null!);

            result.ShouldBeNull();
        }

        [Fact]
        public void Parse_UnknownContentType_ReturnsDefault()
        {
            var content = "some content";

            var result = ResponseParser.Parse<SampleModel>(content, "application/unknown");

            result.ShouldBeNull();
        }

        [Fact]
        public void Parse_JsonWithCustomOptions_RespectsOptions()
        {
            const string json = "{\"id\":1,\"name\":\"Custom\"}";

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };

            var result = ResponseParser.Parse<SampleModel>(json, "application/json", options);

          
            result.ShouldNotBeNull();
            result!.Id.ShouldBe(0); 
            result.Name.ShouldBeNullOrEmpty();
        }
    }
}
