using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Spire;
using System.Data;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using VSP_HealthExam.Web.Entity;
using VSP_HealthExam.Web.Services;
using VSP_HealthExam.Web.Services.BaoHiem;
using VSP_HealthExam.Web.Services.KhamSucKhoe;
using VSP_HealthExam.Web.ViewModels;
using VSP_HealthExam.Web.Ultility;
using VSP_HealthExam.Web.Services.Register;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("SqlConnectionString");
//var connectionStringSql = builder.Configuration.GetConnectionString("SqlConnectionString");

Spire.License.LicenseProvider.SetLicenseKey("JYr60e14h8EN/k7MAQBeHEqRMJqdHy0nDhXO7ZX580ILn533U1pOV4dqiERAh4F6ECjlocb98iVFKV/NpckRLc+TeX6gUHn9nOWbYLqUdsREElUECDhNR7uYYKd4jDYcTwqJ8AzGn4vF+yaLXz9/mgQVTf2nMK+NkNJ3QIx0+e9tlMvCH9OYDk914MiORrGtwWeBqjpycyOjei7EHwFjMMKS9SvoWW1+ADRts8x4srEZ8V23bMSHp4mjI8yLxyITFrQ2UBbrSohWE4Wwj3bukQ/0FJfWjHZqGWpfd7164J0K/p19hlI9bMLJNgfUcc0v6tIx5gsF6gzEPfZZoIVIfyVk2978x7LX+1uGxAWvSwgtp3gBhPfF1b9ieMIUwmFdln0rauKgazy6RHSOHWrevZRtDCnPkiT/EhxEvQiNZriZ+Mqv2p5b8rCU1Qm9mcIrnqV9JQfOWQzVe+K/kbm08mmPGwyZ6QlV6foKY3LtuCtHknmzlEXDQMjsAXD7W4pRklsqLNdXj8V1NhCY2j9uLVA7XL50TkY40YO+h3D7X3r7yvl6wpaem5D7tLQbOkoARv6CkFZFbWAENEcTOyL7yX+CDj9gR9fUHoCqukdqyjT/H6NxyK4ivkKcOFImgg5VAijMYO1x1f2ZZjcuUo6bNQwYJugPFq1ZEUUwekgWb1GOtGgzRFl7wY4N6WwVDmDoG9Bz1nCtl3LyAWa43PnmyWfZSPM0CbVhbyZWU4nPFOIDJufFo1SKq4l5O5OJQK5F+c5QAlRCz/d3+E6oSm6k/tSpFlFr11ldFHqGgJviEybvVJ9mscvlyy7EuigsVI27rf0dmAg6NBFYRoMKGJF0/1BB6+xHklaMadRViaWjImAGqGuhKhzYoTyLM5M6oRZDnFmCvN0QsxxYSW8NzUnNWd6BjFvTB6QzuKEd3VJtfxReEoT5kQDeqRq0oQRvyD0a/0JnR24N3mIb4W7zbrKxfEQKxfVrCch5/qbHDckrOw/e9573tuey8B5w7GyHPswK8tD/xKLvxjUed5bzt+/6MBgA6CiFZDAQDgkaPVxEnRIwMHaXlQkyiIqDERbjI8dI2b2DMxgYOwGfqdheRbHm1NzGKXqF2AV8scN6dhvH3IXh9U35nex4UKU3YV6byHmKIzKq7mbSmOERu0vJ0h6Ak2Ii68cCCS4tefxkTfvppE56cMdnfFcuwzKq2dYcUOlZCNNvMs7AyKj3Xt85PB+GmCZzKtjOHOcArosr4d4tZaYmyUeHoOt0AczfpzAPD7MkOscsib3qvoFLN7dencdxK6kb+FU0YYSO0uqRh3/IxPPPmclJWKYGIgBX6Ur+ZsXTg8mjTFaEg0wNNJMuY+QSxiYWxl0AP5wIn4RGlZl5Eqedxl7+GdCffjrMGmWHhnHafbQ7SLShtbnfiB5Yme3VJt2VvE4I+pgtk9U3fQdC0uRRsPVpvxoh6m/M+nDefR6wyEx7WXkP/kZbR4fr5yC3xs9zxf5oSGICd1ZqQdETRiLH3RV3Z7B15Q==");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddDbContext<SubDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionStringLyLich"));
});
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // Relaxed for local dev
    options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = false;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Events.OnSigningIn = async context =>
    {
        var identity = context.Principal.Identities.First();
        var claims = identity.Claims
            .Where(c => c.Type == "DanhSo" || c.Type == ClaimTypes.Role || c.Type == ClaimTypes.NameIdentifier || c.Type == ClaimTypes.Name
                || c.Type == "UserType" || c.Type == "FullName")
            .ToList();
        identity.Claims.ToList().ForEach(c => identity.RemoveClaim(c));
        claims.ForEach(c => identity.AddClaim(c));
        await Task.CompletedTask;
    };
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 2;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
    options.Limits.MaxRequestHeadersTotalSize = 65536; // 64KB to prevent HTTP 400
});

builder.Services.Configure<FormOptions>(options =>
{
    options.BufferBodyLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddLocalization(option => option.ResourcesPath = "Resources");
var supportedCultures = new[] { "vi", "ru" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("vi")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures)
           .RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

builder.Services.AddMvc()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

// Đã xóa toàn bộ các dòng addScoped, addSingleton, addTransient, using, DI liên quan đến các service/DAO/model đã xóa
builder.Services.AddScoped<IMailServices, MailServices>();
builder.Services.AddScoped<I_Document_BH, Document_BH_Service>();
builder.Services.AddScoped<I_KhamSucKhoe, KhamSucKhoe_Service>();
builder.Services.AddScoped<I_ChuyenDeNuRegister, ChuyenDeNuRegister_Service>();


var app = builder.Build();

// Thêm middleware bảo mật
app.UseSecurityHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseStaticFiles();
app.UseRequestLocalization();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});


app.Run();