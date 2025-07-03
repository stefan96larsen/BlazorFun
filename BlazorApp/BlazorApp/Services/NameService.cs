using BlazorApp.Data;
using BlazorApp.DTO;
using BlazorApp.Services.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlazorApp.Services;

public class NameService : INameService
{
    private readonly IDbContextFactory<BlazorAppContext> _contextFactory;
    private readonly IDbLock _dbLock;
    private readonly ILogger _logger;

    public NameService(IDbContextFactory<BlazorAppContext> contextFactory, ILogger<NameService> logger, IDbLock dbLock)
    {
        _contextFactory = contextFactory;
        _logger = logger;
        _dbLock = dbLock;
    }

    public Response PutName(Name putNameRequest)
    {
        if (putNameRequest.SurName.IsNullOrEmpty() || putNameRequest.LastName.IsNullOrEmpty())
        {
            return new Response
            {
                Errors =
                [
                    new Error
                    {
                        Type = Error.ErrorType.FailedToPut,
                        Reasons =
                            $"{nameof(putNameRequest.SurName)} and {nameof(putNameRequest.LastName)} cannot be empty."
                    }
                ]
            };
        }

        using var context = _contextFactory.CreateDbContext();
        var entityName = putNameRequest.ToEntity();

        lock (_dbLock)
        {
            try
            {
                context.Name.Add(entityName);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                _logger.LogWarning("Name has to be unique: {@Name}", entityName);
                return new Response
                {
                    Errors =
                    [
                        new Error
                        {
                            Type = Error.ErrorType.FailedToPut,
                            Reasons = $"Name already exists: {entityName.SurName} {entityName.LastName}"
                        }
                    ]
                };
            }

            _logger.LogInformation("Successfully put new name: {@Name}", entityName);
            return new Response
            {
                Ok = true
            };
        }
    }

    public Names GetAllNames()
    {
        using var context = _contextFactory.CreateDbContext();

        var names = context.Name.ToList();

        return new Names
        {
            NamesList = names.Select(n => n.ToTypes())
        };
    }

    public Response DeleteName(Name deleteNameRequest)
    {
        using var context = _contextFactory.CreateDbContext();
        var entityToDelete = context.Name.FirstOrDefault(n => n.Id == deleteNameRequest.Id);
        
        if (entityToDelete is null)
        {
            _logger.LogWarning("Unable to find the name: {@Name}", deleteNameRequest);
            return new Response
            {
                Errors =
                [
                    new Error
                    {
                        Type = Error.ErrorType.FailedToDelete,
                        Reasons = $"Unable to find the name: {deleteNameRequest.SurName} {deleteNameRequest.LastName}"
                    }
                ]
            };
        }
        
        lock (_dbLock)
        {
            context.Name.Remove(entityToDelete);
            context.SaveChanges();
        }

        _logger.LogInformation("Successfully deleted name: {@Name}", entityToDelete);
        return new Response
        {
            Ok = true
        };
    }

    public Response DeleteNamesById(List<Name> deleteNamesByIdRequest)
    {
        if (deleteNamesByIdRequest.IsNullOrEmpty())
        {
            _logger.LogWarning($"{nameof(DeleteNamesById)} is null or empty");
            return new Response
            {
                Errors =
                [
                    new Error
                    {
                        Type = Error.ErrorType.FailedToDelete,
                        Reasons = "List of names to delete cannot be empty."
                    }
                ]
            };
        }
        
        using var context = _contextFactory.CreateDbContext();
        var idsToDelete = deleteNamesByIdRequest.Select(n => n.Id).ToList();
        var namesToDelete = context.Name.Where(n => idsToDelete.Contains(n.Id)).ToList();

        lock (_dbLock)
        {
            context.Name.RemoveRange(namesToDelete);
            context.SaveChanges();
        }
        
        _logger.LogInformation("Successfully deleted names: {@DeletedNames}", namesToDelete);
        
        return new Response
        {
            Ok = true
        };
    }
}