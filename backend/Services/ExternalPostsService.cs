using System.Net; //معرفة هل ردّ الـ API هو 404 Not Found
using System.Net.Http.Json; //يساعدنا على تحويل JSON إلى كائن C#
using TodoApi.DTOs;

namespace TodoApi.Services;

public class ExternalPostsService : IExternalPostsService
{
    private readonly HttpClient _httpClient;         //أداة نستخدمها لإرسال طلبات HTTP إلى مواقع وواجهات API خارجية.

    public ExternalPostsService(HttpClient httpClient)
    {
        _httpClient = httpClient; //لإرسال طلبات HTTP
    }
     // send req
    public async Task<ExternalPostDto?> GetPostByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"posts/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)//404
        {
            return null;
        }

        response.EnsureSuccessStatusCode(); //إذا كان الرد خطأ غير 404،مثل 500 هذا السطر إلى رمي استثناء

        return await response.Content.ReadFromJsonAsync<ExternalPostDto>(); //تحويل JSON إلى DTO
    }
}
