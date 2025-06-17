using BlazorApp.DTO;
using BlazorApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace BlazorApp.SignalR.FrontendHub;

public class FrontendHub(INameService nameService) : Hub
{
    public async Task<Response> PutName(Name putNameRequest)
    {
        var response = nameService.PutName(putNameRequest);
        return await Task.FromResult(response);
    }

    public async Task<Names> GetAllNames()
    {
        var response = nameService.GetAllNames();
        return await Task.FromResult(response);
    }

    public async Task<Response> DeleteName(Name deleteNameRequest)
    {
        var response = nameService.DeleteName(deleteNameRequest);
        return await Task.FromResult(response);
    }

    public async Task<Response> DeleteNamesById(List<Name> deleteNamesByIdRequest)
    {
        var response = nameService.DeleteNamesById(deleteNamesByIdRequest);
        return await Task.FromResult(response);
    }
}