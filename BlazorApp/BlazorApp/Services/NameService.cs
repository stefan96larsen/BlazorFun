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

    public Response PutName(Name name)
    {
        if (name.SurName.IsNullOrEmpty() || name.LastName.IsNullOrEmpty())
        {
            return new Response
            {
                Errors =
                [
                    new Error
                    {
                        Type = Error.ErrorType.FailedToPut,
                        Reasons = "SurName and LastName cannot be empty."
                    }
                ]
            };
        }
        
        using var context = _contextFactory.CreateDbContext();
        lock (_dbLock)
        {
            try
            {
                context.Name.Add(name.ToEntity());
                context.SaveChanges();
            } 
            catch (Exception e)
            {
                _logger.LogWarning("{Message}", e.Message);

                return new Response
                {
                    Errors =
                    [
                        new Error
                        {
                            Type = Error.ErrorType.FailedToPut,
                            Reasons = $"Unknown error in method {nameof(PutName)}"
                        }
                    ]
                };
            }

            return new Response
            {
                Ok = true
            };
        }
    }

    public Names GetNames()
    {
        using var context = _contextFactory.CreateDbContext();

        var names = context.Name.ToList();

        return new Names
        {
            NamesList = names.Select(n => n.ToTypes())
        };
    }

    public Response DeleteName(Name name)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();

            var idToDelete = context.Name.Where(n => n.SurName == name.SurName && n.LastName == name.LastName)
                .Select(n => n.Id)
                .SingleOrDefault();

            if (idToDelete == 0)
            {
                return new Response
                {
                    Errors =
                    [
                        new Error
                        {
                            Type = Error.ErrorType.FailedToDelete,
                            Reasons = $"Unable to find the name: {name.SurName} {name.LastName}"
                        }
                    ]
                };
            }

            lock (_dbLock)
            {
                context.Name.Remove(new DbModel.Name
                {
                    Id = idToDelete,
                    SurName = name.SurName,
                    LastName = name.LastName
                });
                context.SaveChanges();
            }

            return new Response
            {
                Ok = true
            };
        }
        catch (Exception e)
        {
            return new Response
            {
                Errors =
                [
                    new Error
                    {
                        Type = Error.ErrorType.FailedToDelete,
                        Reasons = e.Message
                    }
                ]
            };
        }
    }

    public Response DeleteNamesById(long[] ids)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();

            var namesToDelete = context.Name.Where(n => ids.Contains(n.Id)).ToList();

            if (namesToDelete.Count == 0)
            {
                return new Response
                {
                    Errors =
                    [
                        new Error
                        {
                            Type = Error.ErrorType.FailedToDelete,
                            Reasons = "No names found for the provided IDs."
                        }
                    ]
                };
            }

            lock (_dbLock)
            {
                context.Name.RemoveRange(namesToDelete);
                context.SaveChanges();
            }

            return new Response
            {
                Ok = true
            };
        }
        catch (Exception e)
        {
            return new Response
            {
                Errors =
                [
                    new Error
                    {
                        Type = Error.ErrorType.FailedToDelete,
                        Reasons = e.Message
                    }
                ]
            };
        }
    }
}