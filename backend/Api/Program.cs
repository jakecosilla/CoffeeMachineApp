using Application;
using Infrastructure;
using Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Basic request logging
app.Use(async (context, next) =>
{
    var logger = app.Logger;
    logger.LogInformation("Handling {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Finished {Method} {Path} with {StatusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
});

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (feature?.Error != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(feature.Error, "An unhandled exception occurred");

            await context.Response.WriteAsJsonAsync(new
            {
                error = "An internal server error occurred"
            });
        }
    });
});

app.UseCors();
app.UseHttpsRedirection();

// Map endpoints
app.MapCoffeeEndpoints();

app.Run();

public partial class Program { }

