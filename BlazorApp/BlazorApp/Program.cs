using BlazorApp.Components;
using BlazorApp.Services;
using Microsoft.EntityFrameworkCore;
using BlazorApp.Data;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<BlazorAppContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BlazorAppContext") ??
                      throw new InvalidOperationException("Connection string 'BlazorAppContext' not found.")));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();

builder.Services.AddSignalR(options =>
{
    // Set max size to 1 GB for a single SignalR request.
    options.MaximumReceiveMessageSize = 1024L * 1024L * 1024L;
    options.EnableDetailedErrors = true;
});

builder.Services.AddScoped<INameService, NameService>();
builder.Services.AddScoped<IDbLock, DbLock>();
builder.Services.AddBlazorBootstrap();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        if (builder.Environment.IsDevelopment())
        {
            tracing.SetSampler(new AlwaysOnSampler());
        }

        tracing.AddAspNetCoreInstrumentation();
        tracing.AddSource("Microsoft.AspNetCore.SignalR.Server");
        tracing.AddConsoleExporter();
    });

builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());
    
var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<BlazorAppContext>();
db.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly);

app.MapControllers();
app.MapHub<BlazorApp.SignalR.FrontendHub.FrontendHub>("/frontendHub");

app.Run();
