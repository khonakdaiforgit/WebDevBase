using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyApp.Application;
using MyApp.WebMVC.Infrastructure;
using MyApp.WebMVC.Mapping;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(MvcMappingProfile));

builder.Services.AddTransient<RefreshTokenHandler>();

// HttpClient با Refresh Token خودکار
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(AppUrl.GetApiUrl());
})
.AddHttpMessageHandler<RefreshTokenHandler>();


// === JWT Authentication از API ===
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    // مهم: توکن را از کوکی هم بخواند (نه فقط از هدر)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // اول از هدر بگیره
            var token = context.Request.Headers.Authorization
                .FirstOrDefault()?.Replace("Bearer ", "");

            // اگر نبود، از کوکی بگیره
            if (string.IsNullOrEmpty(token))
                token = context.Request.Cookies["access_token"];

            if (!string.IsNullOrEmpty(token))
                context.Token = token;

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();