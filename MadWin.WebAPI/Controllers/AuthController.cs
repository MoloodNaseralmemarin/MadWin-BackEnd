using MadWin.Application.DTOs.Account;
using MadWin.Application.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAccountService _accountService;
    public AuthController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        try
        {
            await _accountService.CreateUserAsync(dto);
            return Ok("ثبت نام با موفقیت انجام شد.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return Ok("Account API is working");
    }

    [HttpGet("ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        return Ok("Account API is working");
    }
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        return Ok("Account API is working");
    }
}
