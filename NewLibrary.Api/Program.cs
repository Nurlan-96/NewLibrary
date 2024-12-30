using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using NewLibrary.Application.Queries.AuthorQueries;
using NewLibrary.Application.Queries.BookQueries;
using NewLibrary.Application.Repositories;
using NewLibrary.Application.Services;
using NewLibrary.Application.Services.AuthorServices;
using NewLibrary.Application.Services.BookServices;
using NewLibrary.Core.Entities;
using NewLibrary.Data.DAL;
using NewLibrary.Infrastructure.Repositories;
using NewLibrary.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()  
                           .AllowAnyMethod()  
                           .AllowAnyHeader());  
});


builder.Services.AddScoped<IBookQuery, BookQuery>();
builder.Services.AddScoped<IAuthorQuery, AuthorQuery>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IEpubService, EpubService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new Exception("Database connection string is not set.");
    options.UseNpgsql(connectionString);
});

var assembliesToScan = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => !a.IsDynamic && a.FullName.StartsWith("NewLibrary"))
    .ToArray();

// Register AutoMapper for all filtered assemblies
builder.Services.AddAutoMapper(assembliesToScan);

// Register FluentValidation for all filtered assemblies
foreach (var assembly in assembliesToScan)
{
    builder.Services.AddValidatorsFromAssembly(assembly);
}

// Add services to the container.
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
        ValidIssuer = config["JWTSettings:Issuer"],
        ValidAudience = config["JWTSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTSettings:Key"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "contents")),
    RequestPath = "/contents"
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.Run();
