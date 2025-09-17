
using AlonBuyumEx.DAL;

namespace AlonBuyumEx.Services
{
    public class DataRepositoryCreator : RepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DataRepositoryCreator(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IDataRepository CreateRepository<IDataRepository>()
        { // GetRequiredService fulfills the role of the IProduct.Operation() method in the Factory pattern
            return _serviceProvider.GetRequiredService<IDataRepository>();
        }
    }
}
