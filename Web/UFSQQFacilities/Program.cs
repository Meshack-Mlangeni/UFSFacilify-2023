using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using UFSQQFacilities.Data;
using UFSQQFacilities.Infrastructure;
using UFSQQFacilities.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IWrapper, Wrapper>();
builder.Services.AddScoped<IUserValidator<User>, CustomUserValidator>();

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbConnection")));

builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(
builder.Configuration.GetConnectionString("UserDbConnection")));


builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 8;
    opts.Password.RequireUppercase = true;
    opts.Password.RequireLowercase = true;
    opts.Password.RequireNonAlphanumeric = true;
    opts.Password.RequireDigit = true;
    opts.User.RequireUniqueEmail = true;  
}).AddEntityFrameworkStores<AppIdentityDbContext>();


var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


SeedData.EnsurePopulated(app);
SeedIdentityData.EnsurePopulated(app);

app.Run();
