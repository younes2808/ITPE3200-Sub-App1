using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RAYS.DAL;
using RAYS.Repositories;
using RAYS.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database
builder.Services.AddDbContext<ServerAPIContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services and repositories
builder.Services.AddControllersWithViews();

// User related services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserSearchRepository, UserSearchRepository>();
builder.Services.AddScoped<UserSearchService>();
// Post related services
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<PostService>();
// Mesage related services
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<MessageService>();  // `ILogger<MessageService>` blir automatisk injisert
// Friend related services
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<FriendService>();
// Comment related services
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<CommentService>();
// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/user/login"; // Redirect to login if not authenticated
        options.AccessDeniedPath = "/user/accessdenied"; // Redirect if access is denied
    });

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();         // Log to console
builder.Logging.AddDebug();           // Log to debugger
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));  // Use config from appsettings.json

var app = builder.Build();

// Configure HTTPS redirection and static files
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Error page for production
    app.UseHsts(); // Enable HTTP Strict Transport Security (HSTS)
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles(); // Enable static file serving
app.UseRouting(); // Enable routing
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();  // Enable authorization middleware

// Configure default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();
