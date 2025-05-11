namespace VeriJson;

public sealed class ShouldContinuation
{
    private readonly string _actual;

    internal ShouldContinuation(string actual)
    {
        _actual = actual;
    }

    public void BeEquivalentTo(string expected)
    {
        JsonAssertor.Assert(_actual, expected);
    }
}
