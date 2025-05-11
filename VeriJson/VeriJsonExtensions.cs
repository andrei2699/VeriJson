namespace VeriJson;

public static class VeriJsonExtensions
{
    public static ShouldContinuation Should(this string actual)
    {
        return new ShouldContinuation(actual);
    }
}
