

using Glossary.BusinessLogic.Services;
using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.DataAccess.AppData;
using Glossary.DataAccess.Entities;
using Glossary.DataAccess.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GlossaryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GlossaryDb")));
builder.Services.AddScoped<IDataSeeder, DataSeeder>();
builder.Services.AddScoped<IDataSeederService, DataSeederService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<GlossaryDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();


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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
