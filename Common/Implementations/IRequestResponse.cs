namespace Common.Implementations
{
    /// <summary>
    /// just a marker to detect requestResponse's consumer
    /// T is return type of consumer
    /// </summary>
    public interface IRequestResponse<T> : IRequestResponse where T : class
    {
    }

    public interface IRequestResponse
    {
    }
}
