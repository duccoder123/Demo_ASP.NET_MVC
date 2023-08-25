using Microsoft.EntityFrameworkCore;
using MVC.DataAccess.Data;
using MVC.DataAccess.Repository;
using MVC.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using MVC.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using MVC.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// thêm dbcontext và kết nối với csdl
builder.Services.AddDbContext<ApplicationDbContext>(options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
// kết nối với libray Stripe ( thanh toán tiền điện tử )
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// thêm chức năng định danh
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
}
);
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "217614084294087";
    options.AppSecret = "35aa944e22fc2c51adbebfffd15025a2";
});
// thêm session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
}
);
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddRazorPages();

// thêm một lifecycle (dependence injection)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
SeedDatabase();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using(var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initializer();
    }
}