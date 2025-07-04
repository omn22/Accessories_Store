using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store.Application.DTOs;
using Store.Application.DTOs;
using Store.Application.Interface.Services;
using Store.Application.OperationResults;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUserDTO)
        {

            var result = await _authService.RegisterAsync(registerUserDTO);

            return Ok("Registration successful, please check your email to confirm.");
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest confirmEmailRequest)
        {
            var result = await _authService.ConfirmEmailAsync(confirmEmailRequest.UserId, confirmEmailRequest.Token);

            if (result.Success)
                return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login (LoginDTO loginDTO)
        {
            var result = await _authService.LoginAsync(loginDTO.Email, loginDTO.Password,loginDTO.RememberMe);
            if (result.Success)
                if (result.Success)
                    return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }
        [HttpPost("ForgetPass")]
        public async Task<IActionResult> ForgetPassword (ForgetPasswordDTO forgetPasswordDTO)
        {
            var result = await _authService.ForgetPassAsync(forgetPasswordDTO);
                 if (result.Success)
                if (result.Success)
                    return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });

        }
        [HttpPost("ResetPass")]
        public async Task<IActionResult> ResetPassword (ResetPasswordDTO resetPasswordDTO)
        {
            var result = await _authService.ResetPassAsync(resetPasswordDTO);
            if (result.Success)
                if (result.Success)
                    return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }

    }
}
