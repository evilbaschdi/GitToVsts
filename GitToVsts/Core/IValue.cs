namespace GitToVsts.Core
{
    public interface IValue<T>
    {
        T Value { get; }
    }
}