
namespace TodoApi.DTOs;

public class ExternalPostDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
}