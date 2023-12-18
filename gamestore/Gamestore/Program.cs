using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ServiceLayer;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
using StoreAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Logging.AddProvider(new FileLoggerProvider(builder.Configuration.GetSection("Logging:File")));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GameStoreDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddAutoMapper(typeof(AutomapperProfile));

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

var section = builder.Configuration.GetSection("Logging:File");
string logFilePath = section.GetValue<string>("Path");

if (File.Exists(logFilePath))
{
    File.WriteAllText(logFilePath, string.Empty);
}

app.UseMiddleware<LoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (errorFeature != null)
            {
                var exception = errorFeature.Error;
                string errorMessage = $"Error: {exception.Message}";
                await context.Response.WriteAsync(errorMessage);
            }
        });
    });
}

// Configure the HTTP request pipeline.
app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
