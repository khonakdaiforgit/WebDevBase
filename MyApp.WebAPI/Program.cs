using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyApp.Application.Helper;
using MyApp.Application.UseCases.User;
using MyApp.Application.Validators;
using MyApp.Domain.Interfaces;
using MyApp.Domain.Interfaces.Common;
using MyApp.Domain.Interfaces.Services;
using MyApp.Infrastructure;
using MyApp.Infrastructure.Repositories;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Services;
using MyApp.Infrastructure.Settings;
using System.Text;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

builder.Services.AddSingleton<MongoMapping>();
builder.Services.AddSingleton<MongoIndexing>();


builder.Services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();

builder.Services.AddScoped<LoginUserUseCase>();
builder.Services.AddScoped<RefreshTokenUseCase>();
builder.Services.AddScoped<GetCurrentUserUseCase>();

builder.Services.AddValidatorsFromAssemblyContaining<UserLoginDtoValidator>();

builder.Services.AddHttpClient();

builder.Services.AddAutoMapper(typeof(MyApp.Application.MappingProfile));

// تنظیم احراز هویت JWT
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"])),
        ClockSkew = TimeSpan.FromSeconds(30) 
    };

    // مدیریت خطاها و لاگ
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Unauthorized: Invalid or expired token.\"}");
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvc", builder =>
    {
        builder.WithOrigins(AppSettingsHelper.GetMvcAddress()) // آدرس MVC
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebDevBase API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter the JWT token (without the Bearer prefix)",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
var app = builder.Build();


var mapping = app.Services.GetRequiredService<MongoMapping>();
mapping.RegisterMappings();

var indexing = app.Services.GetRequiredService<MongoIndexing>();
indexing.RegisterIndexing();

//app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowMvc");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok("API is alive!")).WithName("Health");

app.Run();