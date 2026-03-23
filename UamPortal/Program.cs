using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Linq;
var builder = WebApplication.CreateBuilder(args);

// Add authentication with Microsoft.Identity.Web
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddMicrosoftGraph()
    .AddInMemoryTokenCaches();

// Configure OpenID Connect options
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.SignedOutRedirectUri = "/login";
});

// Add shared infrastructure services
builder.Services.AddSingleton<SharedInfrastructure.Repositories.DatabaseConnectionFactory>();
builder.Services.AddScoped<SharedInfrastructure.Repositories.UserRepository>();
builder.Services.AddScoped<SharedInfrastructure.Repositories.InvitationRepository>();
builder.Services.AddScoped<SharedInfrastructure.Services.SendGridEmailService>();
builder.Services.AddScoped<SharedInfrastructure.Services.MicrosoftGraphService>();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Custom middleware for user type validation
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var userTypeClaim = context.User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
        if (string.IsNullOrEmpty(userTypeClaim))
        {
            // Fetch from Graph if not in claims
            var graphService = context.RequestServices.GetService<SharedInfrastructure.Services.MicrosoftGraphService>();
            if (graphService != null)
            {
                try
                {
                    userTypeClaim = await graphService.GetCurrentUserTypeAsync();
                }
                catch
                {
                    // If Graph call fails, default to Member for safety
                    userTypeClaim = "Member";
                }
            }
        }
        if (userTypeClaim == "Guest")
        {
            context.Response.Redirect("/AccessDenied");
            return;
        }
    }
    await next();
});

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
