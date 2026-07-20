using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Exceptions;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("notfound")]
    public IActionResult ThrowNotFound() => throw new NotFoundException("Test: bulunamadı");

    [HttpGet("boom")]
    public IActionResult ThrowGeneric() => throw new Exception("gizli detay");
}