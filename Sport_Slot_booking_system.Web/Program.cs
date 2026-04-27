using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Sport_Slot_booking_system.Web;
using Sport_Slot_booking_system.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// ✅ Register TokenService first (no dependencies)
builder.Services.AddScoped<TokenService>();

// ✅ Register handler (depends on TokenService + IServiceProvider)
builder.Services.AddScoped<AuthHttpMessageHandler>();

// ✅ Register HttpClient with the handler attached
builder.Services.AddScoped(sp =>
{
    var tokenService = sp.GetRequiredService<TokenService>();
    var handler = new AuthHttpMessageHandler(tokenService, sp)
    {
        InnerHandler = new HttpClientHandler()
    };

    return new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7178/")
    };
});
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//Radzen services
builder.Services.AddScoped<NotificationService>();

//custom created services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>()
);
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<BaseApiService>();
builder.Services.AddScoped<SlotBookingService>();
builder.Services.AddScoped<AuthHttpMessageHandler>();
builder.Services.AddScoped<UserService>();




await builder.Build().RunAsync();
