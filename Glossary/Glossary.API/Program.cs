using Glossary.API.Middlewares;
using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.API.Extensions;


var builder = WebApplication.CreateBuilder(args);

AppConfigurations.ConfigureServices(builder.Services, builder.Configuration);
AppConfigurations.ConfigureCors(builder.Services, builder.Configuration);
AppConfigurations.ConfigureAuthentication(builder.Services, builder.Configuration);
AppConfigurations.ConfigureSwagger(builder.Services);
AppConfigurations.ConfigureMiddlewares(builder.Services);

var app = builder.Build();

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
app.UseCors("cors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
