

using TodoApi.DTOs;

namespace TodoApi.Services;

public interface IExternalPostsService
{                        //لب منشور باستخدام رقمه.
    Task<ExternalPostDto?> GetPostByIdAsync(int id);
}
