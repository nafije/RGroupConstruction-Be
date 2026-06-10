using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Services;
using RGroupConstruction.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseMySql(builder.Configuration.GetConnectionString("Default")!,
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default")!),
        opt => opt.MigrationsAssembly("RGroupConstruction.Migrations"));
});

var app = builder.Build();
app.Run();