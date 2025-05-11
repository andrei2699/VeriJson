namespace VeriJson;

public sealed class JsonAssertionException(IEnumerable<JsonAssertionIssue> message)
    : Exception(message.Select(GetMessage).Aggregate((x, y) => $"{x}\n{y}"))
{
    private static string GetMessage(JsonAssertionIssue x)
    {
        return IsEmpty(x.Path) ? x.Issue : $"{x.Issue} at {x.Path}";
    }

    private static bool IsEmpty(string path)
    {
        return path.Length == 0 || path == "$";
    }
}
