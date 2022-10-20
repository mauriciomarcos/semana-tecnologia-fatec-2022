using API.Identity.ApiServices.Implementations;
using API.Identity.ApiServices.Interfaces;
using API.Identity.Configs;
using API.Identity.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers();
services.AddEndpointsApiExplorer();

services.AddSwaggerConfig();
services.AddIdentityConfig(builder.Configuration);
services.AddTransient<IJwtGenerator, JwtGeneratorProvider>();
services.AddTransient<GlobalExceptionHandlingMiddleware>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCustomExceptionMiddleware();
app.UseAuthorization();
app.MapControllers();
app.Run();