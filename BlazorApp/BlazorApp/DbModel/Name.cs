using System.ComponentModel.DataAnnotations;

namespace BlazorApp.DbModel;

public class Name
{
    [Key]
    public long Id { get; set; }
    
    [StringLength(255, MinimumLength = 1, ErrorMessage = $"The {nameof(SurName)} must be between 1 and 255 characters long.")]
    public required string SurName { get; set; }
    
    [StringLength(255, MinimumLength = 1, ErrorMessage = $"The {nameof(LastName)} must be between 1 and 255 characters long.")]
    public required string LastName { get; set; }
}