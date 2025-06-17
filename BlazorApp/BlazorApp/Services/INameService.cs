using BlazorApp.DTO;

namespace BlazorApp.Services;

public interface INameService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="putNameRequest"></param>
    /// <returns></returns>
    Response PutName(Name putNameRequest);
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Names GetAllNames();
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="deleteNameRequest"></param>
    /// <returns></returns>
    Response DeleteName(Name deleteNameRequest);

    Response DeleteNamesById(List<Name> deleteNamesByIdRequest);
}