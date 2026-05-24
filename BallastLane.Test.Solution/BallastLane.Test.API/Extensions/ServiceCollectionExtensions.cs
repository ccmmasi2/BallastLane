namespace BallastLane.Test.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            //services.AddScoped<ICategoryService, CategoryService>();

            //services.AddScoped<IProductService, ProductService>();

            return services;
        }

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            //services.AddScoped<ICategoryRepository, CategoryRepository>();

            //services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
