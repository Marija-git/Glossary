using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.BusinessLogic.Services;
using Glossary.DataAccess.Repositories.Interfaces;
using Glossary.DataAccess.Repositories;
using Glossary.DataAccess.SeedData;
using Glossary.DataAccess.AppData;
using Glossary.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Glossary.BusinessLogic.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Glossary.API.Middlewares;
using Glossary.API.Profiles;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Glossary.API.Extensions
{
    public static class AppConfigurations
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDataSeederService, DataSeederService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGlossaryTermsRepository, GlossaryTermsRepository>();
            services.AddScoped<IGlossaryTermsService, GlossaryTermsService>();
            services.AddScoped<IForbiddenWordsRepository, ForbiddenWordsRepository>();
            services.AddScoped<IDataSeeder, DataSeeder>();

            services.AddDbContext<GlossaryDbContext>(options =>options.UseNpgsql(configuration.GetConnectionString("GlossaryDb")));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<GlossaryDbContext>();
            services.Configure<GlossarySettings>(configuration.GetSection("GlossarySettings"));
            services.AddControllers();
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        }

        public static void ConfigureCors(IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("cors", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .WithHeaders("Content-Type", "Authorization");
                });
            });
        }

        public static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                 ValidateIssuer = true,
                 ValidateAudience = true,
                 ValidateLifetime = true,
                 ValidateIssuerSigningKey = true,
                 ValidIssuer = configuration["Jwt:Issuer"],
                 ValidAudience = configuration["Jwt:Audience"],
                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
              };
            });

            services.AddAuthorization();
        }
        public static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
             {
                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                        },
                        []
                    }
                });
             });
        }
        public static void ConfigureMiddlewares(IServiceCollection services)
        {
            services.AddScoped<ErrorHandlingMiddleware>();
        }

    }
}
