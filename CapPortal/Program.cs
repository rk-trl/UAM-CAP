using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add authentication with Microsoft.Identity.Web for Microsoft Entra ID External Identities
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

// Configure OpenID Connect options
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.SignedOutRedirectUri = "/login";
});

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSession();

// Register CapPortal repositories and services
builder.Services.AddSingleton<CapPortal.Repositories.DatabaseConnectionFactory>();
builder.Services.AddScoped<CapPortal.Repositories.IUserRepository, CapPortal.Repositories.UserRepository>();
builder.Services.AddScoped<CapPortal.Repositories.IInvitationRepository, CapPortal.Repositories.InvitationRepository>();
builder.Services.AddScoped<CapPortal.Services.InvitationValidationService>();
builder.Services.AddScoped<CapPortal.Services.UserProvisioningService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
