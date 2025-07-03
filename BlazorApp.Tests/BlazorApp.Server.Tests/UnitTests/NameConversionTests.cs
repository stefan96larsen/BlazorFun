using BlazorApp.Services.Conversions;

namespace BlazorApp.Tests.BlazorApp.Server.Tests.UnitTests;

public class NameConversionTests
{
    [Fact]
    [Trait("NameConversion", "ToEntity")]
    public void Should_ConvertToEntity_When_NameProvided()
    {
        // Arrange
        var name = new DTO.Name
        {
            Id = 1,
            SurName = "Doe",
            LastName = "John"
        };

        // Act
        var entity = name.ToEntity();

        // Assert
        Assert.Equal(name.Id, entity.Id);
        Assert.Equal(name.SurName, entity.SurName);
        Assert.Equal(name.LastName, entity.LastName);
    }
    
    [Fact]
    [Trait("NameConversion", "ToTypes")]
    public void Should_ConvertToTypes_When_EntityProvided()
    {
        // Arrange
        var entity = new DbModel.Name
        {
            Id = 1,
            SurName = "Doe",
            LastName = "John"
        };

        // Act
        var name = entity.ToTypes();

        // Assert
        Assert.Equal(entity.Id, name.Id);
        Assert.Equal(entity.SurName, name.SurName);
        Assert.Equal(entity.LastName, name.LastName);
    }
}