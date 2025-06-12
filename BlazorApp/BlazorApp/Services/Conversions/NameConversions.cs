
using BlazorApp.DTO;

namespace BlazorApp.Services.Conversions;

public static class NameConversions
{
    public static DbModel.Name ToEntity(this Name name)
    {
        return new DbModel.Name
        {
            Id = name.Id,
            SurName = name.SurName,
            LastName = name.LastName
        };
    }

    public static Name ToTypes(this DbModel.Name name)
    {
        return new Name
        {
            Id = name.Id,
            SurName = name.SurName,
            LastName = name.LastName
        };
    }
    
}