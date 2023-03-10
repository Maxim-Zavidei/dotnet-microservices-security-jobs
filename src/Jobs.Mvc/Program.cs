using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using Jobs.Mvc.Config;
using Jobs.Mvc.Infrastructure;
using Jobs.Mvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

builder.Services.AddHttpClient<IJobService, JobService>()
.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

builder.Services.Configure<ApiConfig>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt=>
{
    opt.AccessDeniedPath = "/Auth/AccessDenied";
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opt =>
{
    opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.Authority = "https://localhost:7000";
    opt.ClientId = "mvc-client";
    opt.ResponseType = OpenIdConnectResponseType.Code;
    opt.SaveTokens = true;
    opt.ClientSecret = "mvc-client-secret";
    opt.UsePkce = true;

    opt.Scope.Add("address");
    opt.ClaimActions.MapUniqueJsonKey("address", "address");
    opt.Scope.Add("roles");
    opt.ClaimActions.MapUniqueJsonKey("role", "role");
    opt.Scope.Add("jobsapi.scope");
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        RoleClaimType = JwtClaimTypes.Role
    };

    opt.GetClaimsFromUserInfoEndpoint = true;
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
