namespace BlazorApp.Tests.BlazorApp.Server.Tests.ServiceTests;

public class NameServiceTests : AppTest
{
    
    [Fact]
    public void Should_PutName_When_ValidNameProvided()
    {
        // Arrange
        var name = new DTO.Name
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
}