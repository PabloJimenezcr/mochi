using Serilog;
using Microsoft.EntityFrameworkCore;
using Ventas.Persistence.Model;
using Ventas.Application.Queries;
using Ventas.Application.Command;
using Ventas.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from configuration (appsettings.json)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<HmacAuthenticationOptions>(
    builder.Configuration.GetSection(HmacAuthenticationOptions.SectionName));
builder.Services.AddSingleton(TimeProvider.System);

// Register DbContext from appsettings.json
builder.Services.AddDbContext<VentasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VentasConnection")));

// Register application queries and commands
builder.Services.AddScoped<IGetCategoriesQuery, GetCategoriesFromAPIQuery>();
builder.Services.AddScoped<IGetProductsQuery, GetProductsQuery>();
builder.Services.AddScoped<IGetClientsQuery, GetClientsQuery>();
builder.Services.AddScoped<ICreateCategoryCommand, CreateCategoryCommand>();
builder.Services.AddScoped<IUpdateCategoryCommand, UpdateCategoryCommand>();
builder.Services.AddScoped<IDeleteCategoryCommand, DeleteCategoryCommand>();
builder.Services.AddScoped<ICreateClientCommand, CreateClientCommand>();
builder.Services.AddScoped<IUpdateClientCommand, UpdateClientCommand>();
builder.Services.AddScoped<IDeleteClientCommand, DeleteClientCommand>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

try
{
    Log.Information("Starting web host");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ventas API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    // Serilog request logging (logs HTTP requests and exceptions)
    app.UseSerilogRequestLogging();

    // Authenticate every request using the configured HMAC credentials.
    app.UseMiddleware<HmacAuthenticationMiddleware>();

    // Global middleware to catch and log any unhandled exceptions during request processing
    app.Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception processing {Method} {Path}", context.Request?.Method, context.Request?.Path);
            throw;
        }
    });

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
