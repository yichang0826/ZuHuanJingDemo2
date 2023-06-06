using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZuHuanJingDemo2.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ZuHuanJingDemo2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ZuHuanJingDemo2Context") ?? throw new InvalidOperationException("Connection string 'MySQLonAzureConnedtion' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));///Set global authority filter
});

#region verify the user with loginMySQLonAzureConnedtion
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10); // 设置Cookie过期时间为10分钟
        options.SlidingExpiration = true; // 启用滑动过期时间
    });
builder.Services.AddAuthenticationCore();
#endregion

// 添加 Razor Pages 服務
builder.Services.AddRazorPages();

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

#region using the verify code
app.UseAuthentication();
app.UseAuthorization();
#endregion

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
