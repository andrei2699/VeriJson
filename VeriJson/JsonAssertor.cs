using System.Text.Json.Nodes;

namespace VeriJson;

internal static class JsonAssertor
{
    internal static void Assert(string actual, string expected)
    {
        var actualNode = JsonNode.Parse(actual);
        var expectedNode = JsonNode.Parse(expected);

        if (actualNode == null && expectedNode == null)
        {
            return;
        }

        var jsonAssertionIssues = Assert(actualNode, expectedNode).ToList();
        if (jsonAssertionIssues.Count != 0)
        {
            throw new JsonAssertionException(jsonAssertionIssues);
        }
    }

    private static IEnumerable<JsonAssertionIssue> Assert(JsonNode? actual, JsonNode? expected)
    {
        switch (actual)
        {
            case null when expected == null:
                yield break;
            case null:
                yield return JsonAssertionIssue.FromDifferentValues(actual, expected);
                yield break;
        }

        if (expected == null)
        {
            yield return JsonAssertionIssue.FromDifferentValues(actual, expected);
            yield break;
        }

        if (expected.GetValueKind() != actual.GetValueKind())
        {
            yield return JsonAssertionIssue.FromDifferentValues(actual, expected);
            yield break;
        }

        switch (actual)
        {
            case JsonArray actualArray when expected is JsonArray expectedArray:
                foreach (var jsonAssertionIssue in AssertArrayElements(actualArray, expectedArray))
                {
                    yield return jsonAssertionIssue;
                }

                break;
            case JsonObject actualObject when expected is JsonObject expectedObject:
                foreach (
                    var jsonAssertionIssue1 in AssertObjectElements(actualObject, expectedObject)
                )
                {
                    yield return jsonAssertionIssue1;
                }

                break;
            case JsonValue actualValue when expected is JsonValue expectedValue:
                {
                    if (!CompareJsonValues(actualValue, expectedValue))
                    {
                        yield return JsonAssertionIssue.FromDifferentValues(actual, expected);
                    }
                }
                break;
        }
    }

    private static IEnumerable<JsonAssertionIssue> AssertObjectElements(
        JsonObject actualObject,
        JsonObject expectedObject
    )
    {
        foreach (var (key, value) in expectedObject)
        {
            if (actualObject.TryGetPropertyValue(key, out var actualValue))
            {
                foreach (var issue in Assert(actualValue, value))
                {
                    yield return issue;
                }
            }
            else
            {
                yield return JsonAssertionIssue.FromMissingKey(key, expectedObject);
            }
        }
    }

    private static IEnumerable<JsonAssertionIssue> AssertArrayElements(
        JsonArray actualArray,
        JsonArray expectedArray
    )
    {
        if (actualArray.Count != expectedArray.Count)
        {
            yield return JsonAssertionIssue.FromDifferentArrayLengths(actualArray, expectedArray);
        }

        for (var i = 0; i < actualArray.Count; i++)
        {
            foreach (var issue in Assert(actualArray[i], expectedArray[i]))
            {
                yield return issue;
            }
        }
    }

    private static bool CompareJsonValues(JsonValue actual, JsonValue expectedValue)
    {
        if (
            actual.TryGetValue(out string? actualString)
            && expectedValue.TryGetValue(out string? expectedString)
        )
        {
            return actualString == expectedString;
        }

        if (actual.TryGetValue(out int actualInt) && expectedValue.TryGetValue(out int expectedInt))
        {
            return actualInt == expectedInt;
        }

        if (
            actual.TryGetValue(out double actualDouble)
            && expectedValue.TryGetValue(out double expectedDouble)
        )
        {
            return Math.Abs(actualDouble - expectedDouble) < double.Epsilon;
        }

        if (
            actual.TryGetValue(out bool actualBool)
            && expectedValue.TryGetValue(out bool expectedBool)
        )
        {
            return actualBool == expectedBool;
        }

        return false;
    }
}
