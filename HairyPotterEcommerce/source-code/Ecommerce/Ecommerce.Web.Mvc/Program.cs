using Amazon;
using AspNetCoreRateLimit;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Ioc;
using Ecommerce.Infrastructure.Sql.DataAccess;
using Ecommerce.Infrastructure.Sql.Ioc;
using Ecommerce.Web.Mvc.Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System.Globalization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Add logging configurations
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); // Ensure it's set to Information or lower


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureSqlServices(builder.Configuration);
builder.Services.AddScoped<IAccountService, Ecommerce.Application.Services.AccountService>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.LoginPath = "/My/Login";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = "/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15); // cookie validation time
    options.Cookie.Name = "db7RzeAKUBHT7oCltNkFZLLbp51Fg9EfB7hdlhKioZ8"; // name of the cookie saved to user's browsers
});


// Rate Limiting Configuration
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

//builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

builder.Services.AddMvc();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = false;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});





var app = builder.Build();
app.MigrateAndSeed();


//DependencyInjection.ExecuteMigrate(app.Services);


//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}


app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        IHeaderDictionary headers = context.Context.Response.Headers;
        string contentType = headers["Content-Type"];
        if (contentType == "application/x-gzip")
        {
            if (context.File.Name.EndsWith("js.gz"))
            {
                contentType = "application/javascript";
            }
            else if (context.File.Name.EndsWith("css.gz"))
            {
                contentType = "text/css";
            }
            headers.Add("Content-Encoding", "gzip");
            headers["Content-Type"] = contentType;
        };

        // Cache static files for 30 days
        if (!app.Environment.IsDevelopment())
        {
            context.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");
            context.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(30).ToString("R", CultureInfo.InvariantCulture));
        }
    }
});



//app.UseWebMarkupMin();
app.UseIpRateLimiting(); // Add IP rate limiting middleware here
app.UseSession(); // Ensure this is called before UseRouting and custom middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Use custom session timeout middleware
//app.UseMiddleware<SessionTimeoutMiddleware>();



app.MapRazorPages();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();

/*

public class SessionTimeoutMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SessionTimeoutMiddleware> _logger;

    public SessionTimeoutMiddleware(RequestDelegate next, ILogger<SessionTimeoutMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var logger = context.RequestServices.GetService<ILogger<SessionTimeoutMiddleware>>();

        if (context.User.Identity.IsAuthenticated)
        {
            if (context.Session.TryGetValue("LastActivityTime", out byte[] value))
            {
                var lastActivity = new DateTime(BitConverter.ToInt64(value, 0));
                var timeElapsed = DateTime.UtcNow - lastActivity;
                logger?.LogInformation("Last activity was at {LastActivity}, {ElapsedSeconds} seconds ago", lastActivity, timeElapsed.TotalSeconds);

                if (timeElapsed.TotalMinutes > 15)
                {
                    logger?.LogInformation("Session expired. Redirecting to login page.");

                    if (!context.Response.HasStarted)
                    {
                        // Clear any existing session state 
                        context.Session.Clear();

                        // Redirect to the logout action in Accountcontroller
                        context.Response.Redirect("/logout"); 
                        return; // Make sure no further processing is done
                    }
                }
                else
                {
                    // Update the session time if the response has not started
                    context.Session.Set("LastActivityTime", BitConverter.GetBytes(DateTime.UtcNow.Ticks));
                }
            }
            else
            {
                logger?.LogInformation("Initializing last activity time.");
                context.Session.Set("LastActivityTime", BitConverter.GetBytes(DateTime.UtcNow.Ticks));
            }
        }

        await _next(context);
    }
}
*/