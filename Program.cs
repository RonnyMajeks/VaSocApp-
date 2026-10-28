using Microsoft.EntityFrameworkCore;
using VaSocApp_System.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages support
builder.Services.AddRazorPages();

// Enable session (needed to store logged-in user info)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register the real SQL Server Database Context
builder.Services.AddDbContext<VaSocDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VaSocConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();