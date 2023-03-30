namespace SharedCore.Domain.Abstraction.Providers
{
    public interface IRedisCacheProvider
    {
        Task<T?> GetAsync<T>(string? key);
        Task SetAsync<T>(string? key, T? value);
    }
}