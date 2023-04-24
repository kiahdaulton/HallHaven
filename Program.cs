using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HallHaven.Data;
using HallHaven.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using HallHaven.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

//var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
//builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());


//if (builder.Environment.IsProduction())
//{
//    builder.Configuration.AddAzureKeyVault(
//        new Uri($"https://{builder.Configuration["HallHavenvault"]}.vault.azure.net/"),
//        new DefaultAzureCredential());
//}

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

builder.Services.AddTransient<SendStudentEmail>();

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

//app.MapControllerRoute(
//               name: "hideProfile",
//               pattern: "Home/HideProfileAsync",
//               defaults: new { controller = "Home", action = "HideProfileAsync" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
