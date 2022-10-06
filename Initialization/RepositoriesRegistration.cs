using DecidioTestExcersice.Repositiories;

namespace DecidioTestExcersice.Initialization
{
    internal static class RepositoriesRegistration
    {
        internal static IServiceCollection AddMailAddressRepository(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<MailAddressRepository, MailAddressRepository>(serviceProvider =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                string cs = config.GetConnectionString("pg");
                return new MailAddressRepository(cs);
            });
        }
    }
}
