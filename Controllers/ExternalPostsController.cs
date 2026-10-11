using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")] //تحديد مسار الـ Controller
public class ExternalPostsController : ControllerBase
{
    private readonly IExternalPostsService _externalPostsService;

    public ExternalPostsController(IExternalPostsService externalPostsService)
    {
        _externalPostsService = externalPostsService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExternalPostDto>> GetPostById(int id)
    {
        var post = await _externalPostsService.GetPostByIdAsync(id); //جلب البيانات وإرجاع النتيجة

        if (post is null)
        {
            return NotFound();
        }

        return Ok(post);
    }
}
