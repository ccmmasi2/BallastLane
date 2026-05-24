using BallastLane.Test.API.Middleware;

namespace BallastLane.Test.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerDocumentation(
            this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "BallastLane API v1");
                options.RoutePrefix = "swagger";
            });

            return app;
        }

        public static IApplicationBuilder UseApplicationMiddleware(
            this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
