using Frontend.WebUI.Services.Implementation;
using Frontend.WebUI.Services.Interface;
using Frontend.WebUI.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// dùng httpfactory phải có
builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

Constants.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
Constants.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
Constants.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
Constants.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
Constants.OrderAPIBase = builder.Configuration["ServiceUrls:OrderAPI"];
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
	option.ExpireTimeSpan = TimeSpan.FromHours(10);
	option.LoginPath = "/Auth/Login";
	option.AccessDeniedPath = "/Auth/AccessDenied";
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
