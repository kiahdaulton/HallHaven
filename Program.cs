using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HallHaven.Data;
using HallHaven.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("HallHavenContextConnection") ?? throw new InvalidOperationException("Connection string 'HallHavenContextConnection' not found.");

builder.Services.AddDbContext<HallHavenContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<HallHavenUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<HallHavenContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
