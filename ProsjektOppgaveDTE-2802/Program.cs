using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ProsjektOppgaveDTE_2802.AuthProviders;
using Microsoft.AspNetCore.ResponseCompression;
using ProsjektOppgaveDTE_2802.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthenticationCore();
builder.Services.AddSignalR();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpClient>(_ => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7022")
});
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapHub<CommentHub>("/commentHub");
app.MapFallbackToPage("/_Host");

app.Run();
