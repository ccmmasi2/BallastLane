using BallastLane.Test.Application.Services.Implementations;
using BallastLane.Test.Application.Services.Interfaces;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Infrastructure.Authentication;
using BallastLane.Test.Infrastructure.Data;
using BallastLane.Test.Infrastructure.Repositories.Implementations;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;

namespace BallastLane.Test.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthService,     AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService,  ProductService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IInvoiceService,  InvoiceService>();

            services.AddSingleton<CategoryValidator>();
            services.AddSingleton<ProductValidator>();
            services.AddSingleton<CustomerValidator>();
            services.AddSingleton<InvoiceValidator>();
            services.AddSingleton<RegisterValidator>();
            services.AddSingleton<LoginValidator>();

            services.AddSingleton<PasswordHasher>();
            services.AddScoped<JwtTokenGenerator>();

            return services;
        }

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            services.AddSingleton<DbConnectionFactory>();

            services.AddScoped<IUserRepository,     UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository,  ProductRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IInvoiceRepository,  InvoiceRepository>();

            return services;
        }
    }
}
