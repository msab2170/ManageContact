using Microsoft.EntityFrameworkCore;
using AddressManager.Data;
using AspectCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AddressManagerContext>(options =>
{
    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.ClearProviders()));
    options.EnableSensitiveDataLogging(false);
    options.UseSqlServer(builder.Configuration.GetConnectionString("AddressManagerContext")
    ?? throw new InvalidOperationException("Connection string 'AddressManagerContext' not found."));
});


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AdventureWorks.Session";
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString());
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseCookiePolicy();
app.UseSession();
app.UseMiddleware<LoggingMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
