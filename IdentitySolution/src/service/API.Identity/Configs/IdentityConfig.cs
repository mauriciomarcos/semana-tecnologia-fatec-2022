using API.Identity.Configs.Models;
using API.Identity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Identity.Configs
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
        {
            #region | Configuração do EntityFramework Core do Identity |
            services.AddDbContext<ApplicationDbContext>(optionsAction: options =>
                //options.UseSqlServer(configuration.GetConnectionString(name: "DefaultConnection"))
                options.UseInMemoryDatabase("dbIdentityServer")
            );
            #endregion

            #region | Configuração do Identity |
            services.AddDefaultIdentity<IdentityUser>()
               .AddRoles<IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
            #endregion

            var appSettings = ConfigurarClasseAppSettings(services, configuration);

            #region | Configuração da Autenticação - JWT com suporte para JWT Bearer|
            services.AddAuthentication(configureOptions: options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                bearerOptions.RequireHttpsMetadata = true;
                bearerOptions.SaveToken = true;
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    //SymmetricSecurityKey: é criada a partir da chave recuperada do appsettings
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Secret)),

                    // ValidateIssuer = true: valida se as APIs possuem o token emitido por emissor específico
                    ValidateIssuer = true,

                    //ValidateAudience = true: força a validação, onde apenas domínios especificados no ValidAudience serão permitidos
                    ValidateAudience = true,
                    ValidAudience = appSettings.ValidoEm,

                    // Dados dos emissor do token: A própria API Identidade.API
                    ValidIssuer = appSettings.Emissor
                };
            });
            #endregion

            return services;
        }

        private static AppSettings ConfigurarClasseAppSettings(IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");

            // Aqui é adicionada a classe AppSettings nos serviços do ASP.NET CORE, sendo agora disponibilizada
            // para outros módulos da aplicação, podendo ser utilizada via injeção de dependência.
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            return appSettings;
        }
    }
}