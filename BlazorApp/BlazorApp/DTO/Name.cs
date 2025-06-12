namespace BlazorApp.DTO;

public class Name
{
    public long Id { get; set; }
    public string SurName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
}

public class Names
{
    public IEnumerable<Name> NamesList { get; set; } = [];
}