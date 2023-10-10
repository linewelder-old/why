using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Why.Data;
using Why.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(
            builder.Configuration.GetConnectionString("SqliteConnection"));
    }
    else
    {
        options.UseMySQL(
            builder.Configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));
    }
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<User>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    });
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddTransient<SeedData>();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<ApplicationDbContext>()!;
    var seedData = scope.ServiceProvider.GetService<SeedData>()!;

    if (app.Environment.IsDevelopment())
    {
        await context.Database.EnsureCreatedAsync();
        await seedData.InitializeAsync();
    }
    else
    {
        await context.Database.MigrateAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
