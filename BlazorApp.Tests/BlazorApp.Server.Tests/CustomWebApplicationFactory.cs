using BlazorApp.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlazorApp.Tests.BlazorApp.Server.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbFilePath;
    
    public CustomWebApplicationFactory(string dbFilePath)
    {
        _dbFilePath = dbFilePath;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbContextFactory<BlazorAppContext>));
            services.RemoveAll(typeof(DbContextOptions<BlazorAppContext>));

            services.AddDbContextFactory<BlazorAppContext>(options => 
                options.UseSqlite($"Data Source={_dbFilePath};Pooling=false"));
        });
    }
}