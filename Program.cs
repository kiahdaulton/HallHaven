using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HallHaven.Data;
using HallHaven.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using HallHaven.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<HallHavenContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<HallHavenUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<HallHavenContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add get user service
builder.Services.AddHttpContextAccessor();

// Email service
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

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
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
