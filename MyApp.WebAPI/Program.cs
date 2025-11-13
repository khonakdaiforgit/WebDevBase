using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MyApp.Application.Abstractions.Authorization;
using MyApp.Application.Abstractions.Users;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;
using MyApp.Infrastructure.Repositories.Interface.Common;
using MyApp.Infrastructure.Seed;
using MyApp.Infrastructure.Services.Auth;
using MyApp.Infrastructure.Services.Authorization;
using MyApp.Infrastructure.Services.Users;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

//var pack = new ConventionPack
//{
//    new CamelCaseElementNameConvention()
//};
//ConventionRegistry.Register("MyAppConventions", pack, _ => true);

// === 1. Configuration ===
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// === 2. MongoDB Context ===
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var settings = MongoClientSettings.FromConnectionString(config.GetConnectionString("MongoDb")!);
    //settings.GuidRepresentation = GuidRepresentation.Standard;
    return new MongoClient(settings);
});

builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var client = sp.GetRequiredService<IMongoClient>();
    return new MongoDbContext(client, "RestaurantAppDb");
});

// === 3. Repositories ===
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILogEntryRepository, LogEntryRepository>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IGalleryItemRepository, GalleryItemRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<INewsletterRepository, NewsletterRepository>();
builder.Services.AddScoped<IEmailSubscriberRepository, EmailSubscriberRepository>();
builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();

// === 4. Unit of Work ===
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// === 5. Services ===
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

// === 6. JWT Authentication ===
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// === 7. Controllers ===
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RestaurantApp API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// === 8. Middleware ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// === 9. Seed Data ===
using (var scope = app.Services.CreateScope())
{
    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    await SeedData.InitializeAsync(unitOfWork);
}

app.Run();