using BlazorApp.Data;
using BlazorApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Tests.BlazorApp.Server.Tests;

[CollectionDefinition("Sequential", DisableParallelization = true)]
public abstract class AppTest : IDisposable
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly string DbFilePath;
    protected readonly IDbContextFactory<BlazorAppContext> DbContextFactory;
    protected readonly INameService NameService;
    private readonly IServiceScope _scope;
    private bool _disposed;
    
    protected AppTest()
    {
        const string testDbFolder = "../../../BlazorApp.Server.Tests/TestDb";
        
        if (!Directory.Exists(testDbFolder))
            Directory.CreateDirectory(testDbFolder);
        
        DbFilePath = Path.Combine(testDbFolder, "sqlite.db");
            
        Factory = new CustomWebApplicationFactory(DbFilePath);
        _scope = Factory.Services.CreateScope();
        
        var db = _scope.ServiceProvider.GetRequiredService<BlazorAppContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        DbContextFactory = _scope.ServiceProvider.GetRequiredService<IDbContextFactory<BlazorAppContext>>();
        NameService = _scope.ServiceProvider.GetRequiredService<INameService>();
    }
    
    public void Dispose()
    {
        if (_disposed)
            return;
        
        _scope.Dispose();
        Factory.Dispose();
        
        GC.Collect();
        GC.WaitForPendingFinalizers();
        
        if (File.Exists(DbFilePath))
            File.Delete(DbFilePath);

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}