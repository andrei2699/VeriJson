using System.Text.Json;

namespace VeriJson.Tests;

public class ShouldBeEquivalentTests
{
    [Fact]
    public void JsonObjectWithOneIssue_ShouldThrowException()
    {
        var exception = Assert.Throws<JsonAssertionException>(() =>
            "{\"key\":true}".Should().BeEquivalentTo("{\"key\": \"value\"}")
        );

        Assert.Equal("Expected value (String) but got true (True) at $.key", exception.Message);
    }

    [Fact]
    public void JsonObjectWithMultipleIssues_ShouldThrowException()
    {
        var exception = Assert.Throws<JsonAssertionException>(() =>
            """
            {
            "key":true,
            "ok-key": 2,
            "boolean-key": false
            }
            """.Should().BeEquivalentTo(
                """
                {
                   "key": "value",
                   "missing-key": "value",
                   "ok-key": 2,
                   "boolean-key": true
                }
                """
            )
        );

        Assert.Equal(
            "Expected value (String) but got true (True) at $.key\n"
                + "key 'missing-key' was not found\n"
                + "Expected true (True) but got false (False) at $.boolean-key",
            exception.Message
        );
    }

    [Fact]
    public void JsonNestedObjectWithOneIssue_ShouldThrowException()
    {
        var exception = Assert.Throws<JsonAssertionException>(() =>
            """
            {
               "key": {
                   "nested": {
                       "key": 2
                   }
               }
            }
            """.Should().BeEquivalentTo(
                """
                {
                   "key": {
                       "nested": {
                           "key": "2"
                       }
                   }
                }
                """
            )
        );

        Assert.Equal(
            "Expected 2 (String) but got 2 (Number) at $.key.nested.key",
            exception.Message
        );
    }

    [Fact]
    public void JsonArrayWithDifferentSizes_ShouldThrowException()
    {
        var exception = Assert.Throws<JsonAssertionException>(() =>
            "[]".Should().BeEquivalentTo("[1, 2]")
        );

        Assert.Equal("Expected array of length 2 but got 0", exception.Message);
    }

    [Fact]
    public void JsonWithNestedArrayWithDifferentSizes_ShouldThrowException()
    {
        var exception = Assert.Throws<JsonAssertionException>(() =>
            "{\"data\":[]}".Should().BeEquivalentTo("{\"data\": [1, 2]}")
        );

        Assert.Equal("Expected array of length 2 but got 0 at $.data", exception.Message);
    }

    [Fact]
    public void JsonArrayWithDifferentElements_ShouldThrowException()
    {
        var exception = Assert.Throws<JsonAssertionException>(() =>
            "[1, 2, 3]".Should().BeEquivalentTo("[1, 2, 4]")
        );

        Assert.Equal("Expected 4 (Number) but got 3 (Number) at $[2]", exception.Message);
    }

    [Fact]
    public void EmptyJson_WhenKeyIsExpected_ShouldThrowException()
    {
        var exception = Assert.Throws<JsonAssertionException>(() =>
            "{}".Should().BeEquivalentTo("{\"key\": \"value\"}")
        );

        Assert.Equal("key 'key' was not found", exception.Message);
    }

    [Fact]
    public void GivenDifferentValueKinds_ShouldThrowException()
    {
        var exception = Assert.Throws<JsonAssertionException>(() =>
            "1".Should().BeEquivalentTo("true")
        );

        Assert.Equal("Expected true (True) but got 1 (Number)", exception.Message);
    }

    [Fact]
    public void GivenActualWithMoreValues_ThenShouldBeEquivalent()
    {
        """
            {
                "data": [1, 2],
                "data2": [1, 2]
            }
            """.Should().BeEquivalentTo("{\"data\": [1, 2]}");
    }

    [Theory]
    [InlineData("null")]
    [InlineData("2")]
    [InlineData("7.2")]
    [InlineData("0")]
    [InlineData("0.0")]
    [InlineData("-0.0")]
    [InlineData("-2.0")]
    [InlineData("-72")]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("\"text\"")]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("{\"key\": \"value\"}")]
    [InlineData("{\"key\": 2}")]
    [InlineData("{\"key\": 2.0}")]
    [InlineData("{\"key\": true}")]
    [InlineData("{\"key\": false}")]
    [InlineData("{\"key\": null}")]
    [InlineData("{\"key\": {}}")]
    [InlineData("{\"key\": {\"key\": \"value\"}}")]
    [InlineData("{\"key\": {\"key\": 2}}")]
    [InlineData("{\"key\": {\"key\": 2.0}}")]
    [InlineData("{\"key\": {\"key\": true}}")]
    [InlineData("{\"key\": {\"key\": false}}")]
    [InlineData("{\"key\": {\"key\": null}}")]
    [InlineData("{\"key\": {\"key\": {}}}")]
    [InlineData("{\"key\": {\"key\": [{\"key\": \"value\"}, {\"key\": \"value2\"}]}}")]
    [InlineData("[true, false]")]
    [InlineData("[true, false, null]")]
    [InlineData("[true, false, 2]")]
    [InlineData("[2.0, 4.5, -2.6]")]
    [InlineData("[\"text\", \"text2\"]")]
    [InlineData("[{\"key\": \"value\"}, {\"key\": \"value2\"}]")]
    [InlineData("[{\"key\": [{\"key\": \"value\"}, {\"key\": \"value2\"}]}]")]
    public void ShouldBeEquivalent(string json)
    {
        json.Should().BeEquivalentTo(json);
    }

    [Theory]
    [InlineData("")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("[")]
    [InlineData("]")]
    [InlineData("text")]
    public void GivenInvalidJson_ExpectJsonException(string json)
    {
        Assert.ThrowsAny<JsonException>(() => json.Should().BeEquivalentTo(json));
    }
}
