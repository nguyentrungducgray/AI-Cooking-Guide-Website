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

            // Add authentication service
            builder.Services.AddAuthentication("YourCookieScheme")
                .AddCookie("YourCookieScheme", options =>
                {
                    options.LoginPath = "/Login"; // Path to the login page
                    options.LogoutPath = "/Logout"; // Path to the logout page
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
