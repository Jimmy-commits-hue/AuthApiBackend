using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Web.DTOs;
using Web.Exceptions;
using Web.Interfaces;
using Web.Models;
using Web.Security;
using Web.Utilities;

namespace Web.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> _logger;
        private IConfiguration config;
        private readonly GenerateJwtToken _generateJwtToken;
        private readonly IUserService _userService;
        private readonly IEmailService emailService;
        private readonly ISendEmailService sendEmailService;
        private readonly ICustomNumberService _number;
        private readonly IRefreshTokenService _refreshTokenService;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, GenerateJwtToken h,
            IUserService userService, ISendEmailService sendEmailService, ICustomNumberService number,
            IRefreshTokenService refrestToken, IEmailService emailService)
        {

            _logger = logger;
            _generateJwtToken = h;
            this.config = config;
            _userService = userService;
            this.sendEmailService = sendEmailService;
            _number = number;
            _refreshTokenService = refrestToken;
            this.emailService = emailService;

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto User)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guid userId;

            try
            {

                userId = await _userService.CreateUserAsync(User);
                Guid codeId = await emailService.CreateCodeAsync(userId);

                await sendEmailService.SendEmailWithCodeAsync(User.Email, User.FirstName, User.Surname, codeId);

            }
            catch (UserAlreadyExistException ex)
            {

                return BadRequest(ex.Message);

            }
            catch (FailedToSendEmailException ex)
            {

                return BadRequest(ex.Message);

            }

            return NoContent();

        }

        [AllowAnonymous]
        [HttpGet("Verify")]
        public async Task<IActionResult> Verify([FromQuery] Guid codeId)
        {

            EmailVerification? codeInfo = null;

            try
            {

                codeInfo = await emailService.GetCodeAsync(codeId);

            }
            catch (UserNotFoundException ex)
            {

                return BadRequest(ex.Message);

            }

            var userInfo = await _userService.GetUserById(codeInfo.userId);

            if (!TimeHelper.isCodeActive((DateTime)codeInfo.ExpiresAt!, codeInfo.codeStatus))
            {

                await emailService.UpdateCodeAsync(codeInfo);

                try
                {

                    var newCodeId = await emailService.ReCreateCodeAsync(codeInfo.userId);
                    await sendEmailService.SendEmailWithCodeAsync(userInfo.Email, userInfo.FirstName, userInfo.Surname, newCodeId);

                }
                catch (DailyAttemptsReachedException ex)
                {

                    return BadRequest(ex.Message);

                }
                catch (FailedToSendEmailException ex)
                {

                    return BadRequest(ex.Message);

                }

                return BadRequest(new { Message = "Code expired, New code has been sent to your email" });

            }

            var customNumber = await _number.GetNumber();

            await _userService.UpdateUserNumberAsync(customNumber, userInfo);

            await emailService.UpdateCodeAsync(codeInfo);

            try
            {

                await sendEmailService.SendEmailWithNumberAsync(userInfo.Email, userInfo.FirstName, userInfo.Surname, customNumber);
                 
            }
            catch (FailedToSendEmailException ex)
            {

                return BadRequest(ex.Message);

            }

            return Ok(new { Message = "Email Verified Successfully" });

        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDtos login)
        {

            _logger.LogInformation("Login endpoint called.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = new();

            try
            {

                user = (await _userService.GetUserAsync(login))!;

            }
            catch (UserNotFoundException ex)
            {

                return StatusCode(404, ex.Message);

            }
            catch (UserNotActivatedException ex)
            {

                return StatusCode(400, ex.Message);

            }
            catch (InvalidCredentialsException ex)
            {

                return StatusCode(400, ex.Message);

            }

            var tokenString = _generateJwtToken.GenerateToken(user);

            Response.Cookies.Append("accessToken", tokenString, new CookieOptions
            {

                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)

            });

            return Ok(new { tokenString });

        }

        [Authorize]
        [HttpGet("SecureData")]
        public IActionResult GetSecureData()
        {

            _logger.LogInformation("SecureData endpoint called.");
            return Ok(new { HttpContext.User?.Identity?.Name });

        }

    }

}