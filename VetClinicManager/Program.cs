using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.Models;
using VetClinicManager.Services;
using VetClinicManager.Mappers;
using VetClinicManager.Areas.Admin.Mappers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";

    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    options.LogoutPath = "/Identity/Account/Logout";
    
    options.Events.OnSignedIn = async context =>
    {
        if (context.Principal.IsInRole("Admin"))
        {
            context.Response.Redirect("/Admin");
        }
    };
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<IVisitUpdateService, VisitUpdateService>();
builder.Services.AddScoped<IHealthRecordService, HealthRecordService>();
builder.Services.AddScoped<IAnimalMedicationService, AnimalMedicationService>();
builder.Services.AddSingleton<VetClinicManager.Areas.Admin.Mappers.UserMapper>();
builder.Services.AddSingleton<VetClinicManager.Mappers.UserMapper>();
builder.Services.AddSingleton<VisitUpdateMapper>(); 
builder.Services.AddSingleton<MedicationMapper>();
builder.Services.AddSingleton<VisitMapper>();
builder.Services.AddSingleton<AnimalMapper>();
builder.Services.AddSingleton<HealthRecordMapper>(); 
builder.Services.AddSingleton<AnimalMedicationMapper>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddTransient<SeedData>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seedData = scope.ServiceProvider.GetRequiredService<SeedData>();

    try
    {
        await seedData.InitializeAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        dbContext.Database.OpenConnection();
        dbContext.Database.CloseConnection();
        Console.WriteLine("Pomyslnie polaczono z baza danych");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Blad polaczenia: {ex.Message}");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Users}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();