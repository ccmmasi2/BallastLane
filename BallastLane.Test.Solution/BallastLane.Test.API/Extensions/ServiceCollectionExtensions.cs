using BallastLane.Test.Application.Services.Implementations;
using BallastLane.Test.Application.Services.Interfaces;
using BallastLane.Test.Application.Validators;
using BallastLane.Test.Infrastructure.Authentication;
using BallastLane.Test.Infrastructure.Data;
using BallastLane.Test.Infrastructure.Repositories.Implementations;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            return services;
        }

        public static IServiceCollection AddValidators(
            this IServiceCollection services)
        {
            services.AddScoped<CategoryValidator>();
            services.AddScoped<ProductValidator>();
            services.AddScoped<CustomerValidator>();
            services.AddScoped<InvoiceValidator>();
            services.AddScoped<RegisterValidator>();
            services.AddScoped<LoginValidator>();

            return services;
        }

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            services.AddSingleton<DbConnectionFactory>();
            services.AddScoped<DatabaseInitializer>();

            services.AddScoped<IUserRepository,     UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository,  ProductRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IInvoiceRepository,  InvoiceRepository>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var secret   = configuration["Jwt:Secret"]   ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
            var issuer   = configuration["Jwt:Issuer"]   ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
            var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

            services.AddSingleton<PasswordHasher>();
            services.AddScoped<JwtTokenGenerator>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = true,
                        ValidateAudience         = true,
                        ValidateLifetime         = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer              = issuer,
                        ValidAudience            = audience,
                        IssuerSigningKey         = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secret)),
                        ClockSkew                = TimeSpan.Zero
                    };
                });

            return services;
        }
    }
}
