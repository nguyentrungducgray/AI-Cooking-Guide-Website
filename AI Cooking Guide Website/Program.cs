using AI_Cooking_Guide_Website.ModelAI;


namespace AI_Cooking_Guide_Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure HttpClient
            builder.Services.AddScoped<RecipeApiService>();
            builder.Services.AddHttpClient<RecipeApiService>();

            

            builder.Services.AddDistributedMemoryCache();

            // Add authentication service
            builder.Services.AddAuthentication("YourCookieScheme")
                .AddCookie("YourCookieScheme", options =>
                {
                    options.LoginPath = "/Login/Login";  // Path to the login page
                    options.LogoutPath = "/Logout"; // Path to the logout page
                    options.AccessDeniedPath = "/Login/Login"; // Redirect to login if access is denied
                    options.SlidingExpiration = true; // Cookie expiration sliding
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie expiration time
                });
            // Add authorization policy for admin
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("Admin")); // Only users with the "Admin" role can access the admin area
            });

            // Add session services
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
                options.Cookie.HttpOnly = true; // Ensure cookies are accessible only via the server
                options.Cookie.IsEssential = true; // Mark the session cookie as essential
            });
            // Add controllers with views
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Default HSTS value
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Add session middleware to the request pipeline
            app.UseSession();
            

            // Default route for other areas
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
