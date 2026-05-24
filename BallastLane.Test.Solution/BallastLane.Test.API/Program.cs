using BallastLane.Test.API.Extensions;
using BallastLane.Test.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwagger();
builder.Services.AddRepositories();
builder.Services.AddValidators();
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().InitializeAsync();

app.UseHttpsRedirection();
app.UseSwaggerDocumentation();
app.UseApplicationMiddleware();
app.MapControllers();

app.Run();
