namespace GitToVsts.Core
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // ReSharper disable once TypeParameterCanBeVariant
    public interface IValue<T>
    {
        /// <summary>
        ///     Value of type T.
        /// </summary>
        T Value { get; }
    }
}