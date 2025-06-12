using BlazorApp.DTO;
using BlazorApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace BlazorApp.SignalR.FrontendHub;

public class FrontendHub(INameService nameService) : Hub
{
    public async Task<Response> PutName(Name name)
    {
        var response = nameService.PutName(name);
        return await Task.FromResult(response);
    }

    public async Task<Names> GetNames()
    {
        var response = nameService.GetNames();
        return await Task.FromResult(response);
    }

    public async Task<Response> DeleteName(Name name)
    {
        var response = nameService.DeleteName(name);
        return await Task.FromResult(response);
    }

    public async Task<Response> DeleteNamesById(long[] ids)
    {
        var response = nameService.DeleteNamesById(ids);
        return await Task.FromResult(response);
    }
}