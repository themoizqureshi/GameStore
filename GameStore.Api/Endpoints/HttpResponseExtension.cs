using System.Text.Json;

namespace GameStore.Api.Endpoints;

public static class HttpResponseExtension
{
    public static void AddPaginationHeader(
        this HttpResponse response,
        int totalCount,
        int pageSize)
    {
        var paginationHeader = new
        {
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));
    }
}