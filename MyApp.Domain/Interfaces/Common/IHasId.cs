namespace MyApp.Domain.Interfaces.Common
{
    public interface IHasId<T>
    {
        T Id { get; }
    }
}
