using AutoMapper;
using MyApp.WebAPI.Extensions;
using MyApp.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);


// === 1. Configuration ===
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
// 2. MongoDB
builder.Services.AddMongoDb(builder.Configuration);

// 3. AutoMapper + HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(MyApp.Application.AutoMapperProfile));

// 4. همه سرویس‌های اپلیکیشن
builder.Services.AddApplicationServices();

// 5. Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// 6. Swagger
builder.Services.AddSwagger();

// 7. Controllers
builder.Services.AddControllers();

var app = builder.Build();


var mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid(); // ← اگر خطا داد، نشون می‌ده کدوم مپ مشکل داره

// === Middleware ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.UseMiddleware<RequestLoggingMiddleware>();

await app.SeedDataAsync();

app.Run();

//public partial class Program { }