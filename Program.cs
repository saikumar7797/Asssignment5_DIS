using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HealthyMe.Areas.Identity.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("HealthyMeContextConnection") ?? throw new InvalidOperationException("Connection string 'HealthyMeContextConnection' not found.");

builder.Services.AddDbContext<HealthyMeContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<HealthyMeContext>();

// Add the repository service registration
builder.Services.AddScoped<IHealthyMeRepository, HealthyMeRepository>();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
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

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "calorieTracker",
    pattern: "{controller=CalorieTracker}/{action=Index}/{id?}",
    defaults: new { controller = "CalorieTracker", action = "Index" });

app.MapControllerRoute(
    name: "FoodSearch",
    pattern: "{controller=FoodSearch}/{action=Index}/{id?}",
    defaults: new { controller = "FoodSearch", action = "Index" });

app.MapControllerRoute(
    name: "searchFood",
    pattern: "FoodSearch/SearchFood/{query?}",
    defaults: new { controller = "FoodSearch", action = "FoodSearch" });

app.MapControllerRoute(
    name: "selectedFood",
    pattern: "FoodSearch/selectedFood/{query?}",
    defaults: new { controller = "FoodSearch", action = "selectedFood" });

app.MapControllerRoute(
    name: "EditFood",
    pattern: "FoodSearch/EditFood/{id?}",
    defaults: new { controller = "FoodSearch", action = "EditFood" });

app.MapControllerRoute(
    name: "CalorieTrackerDelete",
    pattern: "CalorieTracker/Delete/{id?}",
    defaults: new { controller = "CalorieTracker", action = "Delete" });


app.MapControllerRoute(
    name: "Nutrition",
    pattern: "{controller=Nutrition}/{action=Index}/{id?}",
    defaults: new { controller = "Nutrition", action = "Index" });


app.MapControllerRoute(
    name: "NutritionFoodSearch",
    pattern: "Nutrition/NutritionFoodSearch/{query?}",
    defaults: new { controller = "Nutrition", action = "NutritionFoodSearch" });

app.MapControllerRoute(
    name: "GetNutritionDetails",
    pattern: "Nutrition/GetNutritionDetails/{query?}",
    defaults: new { controller = "Nutrition", action = "GetNutritionDetails" });
app.Run();
