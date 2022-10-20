using API.Identity.Configs.Models;
using Microsoft.OpenApi.Models;

namespace API.Identity.Configs
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name: "v1", new OpenApiInfo
                {
                    Title = "Identity API",
                    Description = "Provê controle de acesso de usuários.",
                    Contact = new OpenApiContact()
                    {
                        Name = "Maurício Marcos",
                        Email = "001.mmarcos@gmail.com"
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licences/MIT")
                    }
                });

                c.EnableAnnotations();
                c.OperationFilter<SwaggerDefaultValues>();
            });

            return services;
        }
    }
}