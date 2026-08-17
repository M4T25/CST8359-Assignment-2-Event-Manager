using EventManager.Data;
using EventManager.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var useInMemoryDatabase = builder.Configuration.GetValue<bool>("Database:UseInMemory")
    || string.IsNullOrWhiteSpace(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useInMemoryDatabase)
    {
        options.UseInMemoryDatabase("EventManagerDevelopment");
        return;
    }

    options.UseSqlServer(connectionString!);
});
builder.Services.AddScoped<IBannerStorageService, BannerStorageService>();
builder.Services.AddControllersWithViews();
if (useInMemoryDatabase)
{
    var keyPath = Path.Combine(Path.GetTempPath(), "EventManager-DataProtection-Keys");
    builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(keyPath));
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.InitializeAsync(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
if (useInMemoryDatabase)
{
    var localUploadsPath = app.Configuration["LocalUploads:Path"]
        ?? Path.Combine(app.Environment.WebRootPath, "uploads");
    Directory.CreateDirectory(localUploadsPath);
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(localUploadsPath),
        RequestPath = "/uploads"
    });
}
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
