using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthApi.DTOs;
using AuthApi.Interfaces;
using AuthApi.Models;
using AuthApi.Security;
using AuthApi.Utilities;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using AuthApi.Exceptions.ExceptionsTypes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AuthApi.Controllers.v1
{

    [ApiController]
    [Route("/api/v{version:apiversion}/[controller]")]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> _logger;
        private readonly GenerateJwtToken _generateJwtToken;
        private readonly IUserService _userService;
        private readonly IEmailService emailService;
        private readonly ICustomNumberService _number;
        private readonly IRefreshTokenService _refreshTokenService;

        public HomeController(ILogger<HomeController> logger, GenerateJwtToken h,IUserService userService,
            ICustomNumberService number, IRefreshTokenService refrestToken, IEmailService emailService)
        {

            _logger = logger;
            _generateJwtToken = h;
            _userService = userService;
            _number = number;
            _refreshTokenService = refrestToken;
            this.emailService = emailService;

        }

        /// <summary>
        /// Registration point for new users
        /// </summary>
        /// <param name="User">User Registration Data</param> 
        /// <response code="200">Ok - Returned if user data has been successfully captured</response>
        /// <response code="400">BadRequest - Returned if the ModelState is not valid</response>
        /// <response code="400">BadRequest - If User Id Number already exist in the database</response>
        /// <exception cref="UserAlreadyExistException">If user already exist</exception>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto User)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


                Guid userId = await _userService.CreateUserAsync(User, default);
                await emailService.CreateCodeAsync(userId);

            return Ok(new
            {
                Message = "Please check for verification email from your gmail and enter the " +
                "verification code sent to you"
            });

        }

        [AllowAnonymous]
        [HttpPost("Verify")]
        public async Task<IActionResult> Verify([FromBody] string code)
        {

            EmailVerification? codeInfo = null;

            try
            {

                codeInfo = await emailService.GetCodeAsync(code);

            }
            catch (UserNotFoundException ex)
            {

                return BadRequest(ex.Message);

            }

            var userInfo = await _userService.GetUserById(codeInfo.userId, default);

            if (!TimeHelper.isCodeActive((DateTime)codeInfo.ExpiresAt!, codeInfo.isActive))
            {

                await emailService.UpdateCodeAsync(codeInfo);

                try
                {

                    var newCodeId = await emailService.ReCreateCodeAsync(codeInfo.userId);

                }
                catch (DailyAttemptsReachedException ex)
                {

                    return BadRequest(ex.Message);

                }
                

                return BadRequest(new { Message = "Code expired, New code has been sent to your email" });

            }
            //generate custom number
            var customNumber = await _number.GetNumber();

            //update user table, allocating login custom number to verified user
            await _userService.UpdateUserNumberAsync(customNumber, userInfo);

            //mark code as inactive
            await emailService.UpdateCodeAsync(codeInfo);

            return Ok(new { Message = "Email Verified Successfully" });

        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDtos login)
        {

            _logger.LogInformation("Login endpoint called.");

            //check model state
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = new();

            try
            {

                user = (await _userService.GetUserAsync(login, default))!;

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

            //generate jwt token
            var tokenString = _generateJwtToken.GenerateToken(user);

            //send the token as a cookie in a http response
            Response.Cookies.Append("accessToken", tokenString, new CookieOptions
            {

                HttpOnly = true, //prevent javascript access
                Secure = true, //only sent over https
                SameSite = SameSiteMode.Strict, //prevent cross-site request forgery
                Expires = DateTimeOffset.UtcNow.AddMinutes(30) //set expiration time

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