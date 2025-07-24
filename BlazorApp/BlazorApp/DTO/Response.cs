namespace BlazorApp.DTO;

public class Error
{
    public enum ErrorType
    {
        /// <summary>
        ///     Failed to put data to the database.
        /// </summary>
        FailedToPut,
        
        /// <summary>
        ///     Faield to delete data from the database.
        /// </summary>
        FailedToDelete
    }
    
    public ErrorType Type { get; set; }
    
    public string Reasons { get; set; } = string.Empty;
    
}

public class Response
{
    /// <summary>
    ///     Defined if the request was successful.
    /// </summary>
    public bool Ok { get; set; }
    
    /// <summary>
    ///     An error if the request failed.
    /// </summary>
    public Error[]? Errors { get; set; }
}