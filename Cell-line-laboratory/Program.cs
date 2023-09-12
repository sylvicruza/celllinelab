using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cell_line_laboratory.Data;
using Cell_line_laboratory.Services;
using Cell_line_laboratory.Job;
using Cell_line_laboratory.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using ArtisanELearningSystem.Services;
using OfficeOpenXml;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Cell_line_laboratoryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Local") ?? throw new InvalidOperationException("Connection string 'Cell_line_laboratoryContext' not found.")));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})

.AddCookie(options =>
{
    options.Cookie.Name = "Celllinecookies";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});





//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<Cell_line_laboratoryContext>()
//    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddHostedService<DailyCellLineStatusCheckService>();
builder.Services.AddHostedService<DataCleanupWorker>();
builder.Services.AddHostedService<PositionNotificationService>();
builder.Services.AddSingleton<ReminderJob>();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//builder.Services.AddHostedService<DataCleanupWorker>();

builder.Services.AddQuartz(q =>
{

    q.AddJob<ReminderJob>(j => j
        .WithIdentity("reminderJob")
        .StoreDurably()
    );

    q.AddTrigger(t => t
        .WithIdentity("reminderTrigger")
        .ForJob("reminderJob")
        .WithCronSchedule("0 0 18 ? * MON-FRI") // Send emails at 6:00 PM on weekdays
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var dataCleanupWorker = services.GetRequiredService<DataCleanupWorker>();
        var notificationService = services.GetRequiredService<PositionNotificationService>();
        var lifetime = services.GetRequiredService<IHostApplicationLifetime>();


        lifetime.ApplicationStarted.Register(() =>
        {
            dataCleanupWorker.StartAsync(lifetime.ApplicationStopping);
        });

        lifetime.ApplicationStopping.Register(() =>
        {
            dataCleanupWorker.StopAsync(CancellationToken.None).Wait();
        });
    

        lifetime.ApplicationStarted.Register(() =>
        {
            notificationService.StartAsync(lifetime.ApplicationStopping);
        });

        lifetime.ApplicationStopping.Register(() =>
        {
            notificationService.StopAsync(CancellationToken.None).Wait();
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while starting the PositionNotificationService.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Make sure this middleware is added before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LandingPage}/{id?}");

app.Run();

