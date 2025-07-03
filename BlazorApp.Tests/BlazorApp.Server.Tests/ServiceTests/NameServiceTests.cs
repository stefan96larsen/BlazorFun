using BlazorApp.DTO;

namespace BlazorApp.Tests.BlazorApp.Server.Tests.ServiceTests;

public class NameServiceTests : AppTest
{
    
    [Fact]
    [Trait("NameService","PutName")]
    public void Should_PutName_When_ValidNameProvided()
    {
        // Arrange
        var name = new Name
        {
            SurName = "Doe",
            LastName = "John"
        };
        
        // Act
        var response = NameService.PutName(name);
    
        // Assert
        using var context = DbContextFactory.CreateDbContext();
        Assert.True(response.Ok);
        Assert.Equal(context.Name.First().SurName, name.SurName);
        Assert.Equal(context.Name.First().LastName, name.LastName);
    }
    
    [Fact]
    [Trait("NameService","PutName")]
    public void Should_FailToPutName_When_NameAlreadyExists()
    {
        // Arrange
        var name = new Name
        {
            SurName = "Doe",
            LastName = "John"
        };
        
        // Act
        var response1 = NameService.PutName(name);
        var response2 = NameService.PutName(name);
    
        // Assert
        using var context = DbContextFactory.CreateDbContext();
        Assert.True(response1.Ok);
        Assert.False(response2.Ok);
        Assert.NotEmpty(response2.Errors!.First().Reasons);
    }

    [Fact]
    [Trait("NameService", "GetAllNames")]
    public void Should_GetAllNames_When_Called()
    {
        // Arrange
        var name1 = new Name
        {
            SurName = "John",
            LastName = "Doe"
        };

        var name2 = new Name
        {
            SurName = "Jane",
            LastName = "Smith"
        };
        
        NameService.PutName(name1);
        NameService.PutName(name2);

        var expectedResponse = new Names
        {
            NamesList = new List<Name>
            {
                new()
                {
                    Id = 1,
                    SurName = "John",
                    LastName = "Doe"
                },
                new()
                {
                    Id = 2,
                    SurName = "Jane",
                    LastName = "Smith"
                }
            }
        };
        
        // Act
        var response = NameService.GetAllNames();
        
        // Assert
        Assert.Equivalent(response.NamesList, expectedResponse.NamesList, strict:true);
    }

    [Fact]
    [Trait("NameService", "DeleteName")]
    public void Should_DeleteName_When_Called()
    {
        // Arrange
        var name = new Name
        {
            SurName = "Doe",
            LastName = "John"
        };
        
        NameService.PutName(name);

        var expectedResponse = new Response
        {
            Ok = true
        };

        var getAllNames = NameService.GetAllNames();
        
        // Act
        var actualResponse = NameService.DeleteName(getAllNames.NamesList.First());
        
        // Assert
        using var context = DbContextFactory.CreateDbContext();
        Assert.Equal(expectedResponse.Ok, actualResponse.Ok);
        Assert.False(context.Name.Any(n => n.SurName == name.SurName && n.LastName == name.LastName));
    }
    
    [Fact]
    [Trait("NameService", "DeleteName")]
    public void Should_FailToDeleteName_When_NameIsNull()
    {
        // Arrange
        // Act
        var actualResponse = NameService.DeleteName(new Name());
        
        // Assert
        Assert.False(actualResponse.Ok);
        Assert.NotEmpty(actualResponse.Errors!.First().Reasons);
    }
    
    [Fact]
    [Trait("NameService", "DeleteNamesById")]
    public void Should_DeleteNamesById_When_Called()
    {
        // Arrange
        var name1 = new Name
        {
            SurName = "Doe",
            LastName = "John"
        };
        
        var name2 = new Name
        {
            SurName = "Smith",
            LastName = "Jane"
        };
        
        NameService.PutName(name1);
        NameService.PutName(name2);

        var namesToDelete = new List<Name>
        {
            new() { Id = 1 },
            new() { Id = 2 }
        };

        // Act
        var response = NameService.DeleteNamesById(namesToDelete);
        
        // Assert
        using var context = DbContextFactory.CreateDbContext();
        Assert.True(response.Ok);
        Assert.Empty(context.Name);
    }
    
    [Fact]
    [Trait("NameService", "DeleteNamesById")]
    public void Should_FailToDeleteNamesById_When_EmptyList()
    {
        // Arrange
        // Act
        var response = NameService.DeleteNamesById([]);
        
        // Assert
        using var context = DbContextFactory.CreateDbContext();
        Assert.False(response.Ok);
        Assert.Empty(context.Name);
    }
}