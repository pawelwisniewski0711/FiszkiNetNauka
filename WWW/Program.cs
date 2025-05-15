using FiszkiNetNauka;
using FiszkiNetNauka.Components;
using FiszkiNetNauka.Components.Account;
using FiszkiNetNauka.Components.FishCardsGenerator.Services;
using FiszkiNetNauka.Models;
using FiszkiNetNauka.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaukaFiszek.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<FiszkiNetDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<FiszkiNetDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Konfiguracja cookie dla autentykacji
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Konfiguracja antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "CSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Dodanie kontrolerów API
builder.Services.AddControllers();

builder.Services.AddHttpClient("OpenRouter", client =>
{
    client.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
#if DEBUG
    // During development and tests, use the example API key
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKeyExample.Key}");
#else
    // In production, use the real API key
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey.Key}");
#endif
});

// Rejestracja serwisów
builder.Services.AddScoped<IOpenRouterService, OpenRouterService>();
builder.Services.AddScoped<IFlashcardGeneratorService, FlashcardGeneratorService>();
builder.Services.AddScoped<IFlashcardService, FlashcardService>();
builder.Services.AddScoped<IFlashcardManagementService, FlashcardManagementService>();

// Dodanie HttpClient dla komponentów Blazor
builder.Services.AddHttpClient();

// Upewnij się, że SignInManager jest dostępny
builder.Services.AddScoped<SignInManager<ApplicationUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Dodanie middleware autoryzacji i autentykacji
app.UseAuthentication();
app.UseAuthorization();

// Dodanie obsługi API
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
