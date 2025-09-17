namespace AlonBuyumEx.Services
{
    public interface ICacheProvider
    {
        // returns an object for the service to use for cache operations
        object GetCache();
    }
}
