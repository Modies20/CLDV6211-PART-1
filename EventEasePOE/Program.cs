using EventEase.Data;
using EventEase.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer; // Add this using directive


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<IBlobService, AzureBlobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Apply EF Core migrations at startup (safer when using migrations)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log the migration failure but do not rethrow so the app can start in development.
        // In production you should fail loudly and investigate the underlying issue.
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database migration failed on startup. Application will continue running, but DB may be out of sync.");
        Console.Error.WriteLine(ex);
        // Optionally, you could fallback to EnsureCreated in development environments only.
        // dbContext.Database.EnsureCreated();
    }
}

app.Run();