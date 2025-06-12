using BlazorApp.DTO;

namespace BlazorApp.Services;

public interface INameService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Response PutName(Name name);
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Names GetNames();
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Response DeleteName(Name name);

    Response DeleteNamesById(long[] ids);
}