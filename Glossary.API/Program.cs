

using Glossary.API.Middlewares;
using Glossary.API.Profiles;
using Glossary.BusinessLogic.Services;
using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.DataAccess.AppData;
using Glossary.DataAccess.Entities;
using Glossary.DataAccess.Repositories;
using Glossary.DataAccess.Repositories.Interfaces;
using Glossary.DataAccess.SeedData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IDataSeederService, DataSeederService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGlossaryTermsRepository, GlossaryTermsRepository>();
builder.Services.AddScoped<IGlossaryTermsService, GlossaryTermsService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddDbContext<GlossaryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GlossaryDb")));
builder.Services.AddScoped<IDataSeeder, DataSeeder>();
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<GlossaryDbContext>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddEndpointsApiExplorer();
//Swagger konfiguracija za JWT
builder.Services.AddSwaggerGen(c =>
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


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        var seedService = scope.ServiceProvider.GetRequiredService<IDataSeederService>();
        await seedService.SeedAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during seeding: {ex.Message}");
    }

}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
