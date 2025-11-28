using Comments.Core.Dtos;
using Comments.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Comments.Core.Services;

public class PostService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IPostService
{
    public async Task<PostDto?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await httpClientFactory.CreateClient().GetAsync($"{configuration["Services:Posts"]}/api/v1/{guid}", cancellationToken);

        if (!httpResponseMessage.IsSuccessStatusCode) return null;

        return JsonConvert.DeserializeObject<PostDto>(await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));
    }
}