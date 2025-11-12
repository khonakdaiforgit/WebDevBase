using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using MyApp.Application.Helper;
using MyApp.WebMVC.Handlers;
using MyApp.WebMVC.Middleware;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// تنظیم Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
    .SetApplicationName("MyApp")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options => { options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")); });


builder.Services.AddHttpClient("publicApi", client =>
{
    client.BaseAddress = new Uri($"{AppSettingsHelper.GetApiAddress()}api/"); // آدرس Web API
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri($"{AppSettingsHelper.GetApiAddress()}api/"); // آدرس Web API
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<JwtTokenHandler>();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<JwtTokenHandler>();

// Configure cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "MyApp.Auth"; // اطمینان از نام ثابت
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // برای localhost
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(AppSettingsHelper.refreshTokenExpireAfterMints);
        options.SlidingExpiration = true;
        options.CookieManager = new ChunkingCookieManager();
        options.Events.OnValidatePrincipal = context =>
        {
            Console.WriteLine($"OnValidatePrincipal: IsAuthenticated = {context.Principal.Identity.IsAuthenticated}");
            Console.WriteLine($"Claims: {string.Join(", ", context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddAutoMapper(typeof(MyApp.WebMVC.MappingProfile));


var app = builder.Build();


// پیکربندی Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllerRoute(
    name: "fileSale",
    pattern: "/{uniqueCode:regex(^[[A-Z0-9]]{{10}}$)}",
    defaults: new { controller = "Home", action = "FileSale" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
