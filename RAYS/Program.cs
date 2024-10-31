using RAYS.DAL;
using RAYS.Repositories;
using RAYS.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Konfigurer database
builder.Services.AddDbContext<ServerAPIContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Legg til servicer og repository
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<CommentService>(); // Legg til CommentService
builder.Services.AddScoped<UserService>();    // Legg til UserService
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Legg til UserRepository med IUserRepository-grensesnittet

// Konfigurer session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sett timeout for session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Konfigurer logging i henhold til appsettings.json eller egne standarder
builder.Logging.ClearProviders();  // Rens default-providers for å unngå duplikater
builder.Logging.AddConsole();      // Legg til Console-logging
builder.Logging.AddDebug();        // Legg til Debug-logging (spesielt nyttig i VS)
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));  // Bruker loggnivåer fra appsettings.json

var app = builder.Build();

// Konfigurer HTTP-pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseSession(); // Legg til session-middleware

// Konfigurer standardrute
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
