using System.Text.Json.Nodes;

namespace VeriJson;

public sealed record JsonAssertionIssue
{
    public string Issue { get; }
    public string Path { get; }

    private JsonAssertionIssue(string issue, string path)
    {
        Issue = issue;
        Path = path;
    }

    public static JsonAssertionIssue FromDifferentValues(JsonNode? actual, JsonNode? expected)
    {
        return new JsonAssertionIssue(
            $"Expected {expected} ({expected?.GetValueKind()}) but got {actual} ({actual?.GetValueKind()})",
            GetPath(actual, expected)
        );
    }

    private static string GetPath(JsonNode? actual, JsonNode? expected)
    {
        if (expected is not null)
        {
            return expected.GetPath();
        }

        return actual is not null ? actual.GetPath() : "";
    }

    public static JsonAssertionIssue FromDifferentArrayLengths(
        JsonArray actualArray,
        JsonArray expectedArray
    )
    {
        return new JsonAssertionIssue(
            $"Expected array of length {expectedArray.Count} but got {actualArray.Count}",
            expectedArray.GetPath()
        );
    }

    public static JsonAssertionIssue FromMissingKey(string key, JsonObject expectedObject)
    {
        return new JsonAssertionIssue($"key '{key}' was not found", expectedObject.GetPath());
    }
}
