using AlonBuyumEx.Services;

namespace AlonBuyumEx.DAL
{
    public abstract class RepositoryFactory
    {
        // IServiceProvider is the equivalent of the IProduct in the Factory pattern
        private readonly IServiceProvider _serviceProvider;
                           
        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }                                      
        public abstract T CreateRepository<T>(); // this is the Factory method
       
    }
}
