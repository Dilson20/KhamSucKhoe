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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    });
});
builder.Services.AddDbContext<SubDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionStringLyLich"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    });
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Ensure DB schema is up-to-date (optional)
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate(); // or await db.Database.MigrateAsync();

        // Run your seeding/initialization
        DbInitializer.InitializeAsync(services).GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
        throw;
    }
}

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