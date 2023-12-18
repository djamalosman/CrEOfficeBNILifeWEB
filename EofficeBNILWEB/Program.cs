

using EofficeBNILWEB.DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using EofficeBNILWEB.Models;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

//Translate
builder.Services.AddLocalization(option => { option.ResourcesPath = "Recursos"; });
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(
    option =>
    {
        var languangeTranslate = new List<CultureInfo>
        {
            new CultureInfo("id"),
            new CultureInfo("en")
        };
        option.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("id");
        option.SupportedCultures = languangeTranslate;
        option.SupportedUICultures = languangeTranslate;
    }
);


// Add services to the container.
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Events = new CookieAuthenticationEvents()
        {
            OnRedirectToLogin = redirectContext =>
            {
                var uri = redirectContext.RedirectUri;
                UriHelper.FromAbsolute(uri, out var scheme, out var host, out var path, out var query, out var fragment);
                uri = UriHelper.BuildAbsolute(scheme, host, path);
                redirectContext.Response.Redirect(uri);
                return Task.CompletedTask;
            }
        };
        option.ExpireTimeSpan = TimeSpan.FromHours(3);
        option.LoginPath = "/";
        option.AccessDeniedPath = "/";
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IDataAccessProvider, DataAccessProvider>();
builder.Services.Configure<FtpSettings>(configuration.GetSection("FtpSetting"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseBrowserLink();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.UseRequestLocalization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
