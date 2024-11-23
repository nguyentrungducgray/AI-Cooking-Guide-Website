using AI_Cooking_Guide_Website.ModelAI;

namespace AI_Cooking_Guide_Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình HttpClient
            // Add services to the DI container.
            builder.Services.AddScoped<RecipeApiService>();
            builder.Services.AddHttpClient<RecipeApiService>();

       

            // Thêm dịch vụ xác thực
            builder.Services.AddAuthentication("YourCookieScheme")
                .AddCookie("YourCookieScheme", options =>
                {
                    options.LoginPath = "/Login"; // Đường dẫn đến trang đăng nhập
                    options.LogoutPath = "/Logout"; // Đường dẫn đến trang đăng xuất
                });
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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
      
    }
}
