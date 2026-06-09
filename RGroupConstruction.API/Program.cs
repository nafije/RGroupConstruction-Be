using RGroupConstruction.Application.Common.Constants;
using RGroupConstruction.Application.DTOs.ApiResponse;
using RGroupConstruction.Application.Extensions;
using RGroupConstruction.Domain.Common.Constants;
using RGroupConstruction.Infrastructure.Extentions;
using RGroupConstruction.Infrastructure.Hubs;
using RGroupConstruction.Infrastructure.Seed;
using RGroupConstruction.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Serilog;
using System.Globalization;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddSerilogLogging();

    var localizationSection = builder.Configuration.GetSection(ConfigurationKeys.Sections.Localization);
    SupportedLanguages.Configure(
        localizationSection["DefaultCulture"],
        localizationSection.GetSection("SupportedCultures").Get<string[]>());

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(CorsPolicies.AllowAll, policy =>
        {
            policy.AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .SetIsOriginAllowed(_ => true);
        });
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(SystemPolicies.Admin, policy =>
            policy.RequireRole(SystemPolicies.Admin));
    });

    // Add services to the container.
    builder.Services.AddOpenApi();
    builder.Services.AddLocalization();
    builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
     });


    builder.Services.AddHttpContextAccessor();
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddSwaggerGen();


    var app = builder.Build();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.Info("Starting RGroupConstruction API");

    using (var scope = app.Services.CreateScope())
    {
        await DatabaseSeeder.SeedAllAsync(scope.ServiceProvider);
    }

    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionFeature?.Error;

            if (exception is not null)
            {
                logger.Error(
                    exception,
                    "Unhandled exception for {Method} {Path} with trace id {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(
                ApiResponse.FailureResponse("An unexpected error occurred."));
        });
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/openapi/v1.json", "RGroupConstruction API");
        });
    }
    var supportedCultures = new[] { new CultureInfo("sq"), new CultureInfo("en") };
    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("sq"),
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures,
        RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            }
    });

    app.UseCors(CorsPolicies.AllowAll);
    app.UseStaticFiles();
    app.UseConfiguredSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<NotificationHub>("/hubs/notifications");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "RGroupConstruction API terminated unexpectedly");
    Environment.ExitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}

