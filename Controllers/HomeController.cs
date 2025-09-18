using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthApi.DTOs;
using AuthApi.Exceptions;
using AuthApi.Interfaces;
using AuthApi.Models;
using AuthApi.Security;
using AuthApi.Utilities;

namespace AuthApi.Controllers
{
    /// <summary>
    /// Provides endpoints for user registration, verification, login, and secure data access.
    /// </summary>
    /// <remarks>This controller handles user-related operations such as creating new users, verifying email
    /// codes, logging in,  and accessing secure data. It integrates with various services to perform these operations,
    /// including user  management, email handling, and token generation.</remarks>
    [ApiController]
    [Route("/api/[controller]")]
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
        /// Create user based on user registration data
        /// </summary>
        /// <param name="User">user registration data</param>
        /// <returns>
        /// Returns <see cref="OkResult"/> with a message if the user is created successfully, otherwise,
        /// a <see cref="BadRequestObjectResult"/> with the error message or <seealso cref="BadRequestResult"/>
        /// with error message
        /// </returns>
        /// <remarks>
        ///  This checks for <see cref="ModelState"/> before attempting to create the user
        ///  
        /// <code>
        ///   Guid userId = await _userService.CreateUserAsync(User);
        ///   await emailService.CreateCodeAsync(userId);
        /// </code>
        /// 
        /// <para>
        /// Creates a new user in the database, and generates a verification code in the database
        /// </para>
        /// 
        /// </remarks>
        /// <exception cref="UserAlreadyExistException">if user already exist in the database</exception>
        /// <exception cref="FailedToRandomizeCodeException"> if it failed to generate a unique code 
        /// to be stored into the database
        /// </exception>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto User)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                Guid userId = await _userService.CreateUserAsync(User);
                await emailService.CreateCodeAsync(userId);

            }
            catch (UserAlreadyExistException ex)
            {

                return BadRequest(ex.Message);

            }

            return Ok(new
            {
                Message = "Please check for verification email from your gmail and enter the " +
                "verification code sent to you"
            });

        }

        /// <summary>
        /// It verifies user access code sent to them via email
        /// </summary>
        /// <param name="code">verification code</param>
        /// <returns>
        /// Returns <see cref="OkResult"/> with a message if code verification was successful,
        /// otherwise, <seealso cref="BadRequestResult"/> with the error message
        /// </returns>
        /// <remarks>
        /// 
        ///  <para>
        /// The verification process involves several steps:
        /// </para>
        /// 
        /// <code>
        ///   codeInfo = await emailService.GetCodeAsync(code)
        /// </code>
        /// It retrieves the code from the database using the code as a search key
        /// 
        /// <code>
        ///   var userInfo = await _userService.GetUserById(codeInfo.userId);
        /// </code>
        /// It retrieves the user information associated with the code from the User table in the database
        /// 
        /// <code>
        ///  !TimeHelper.isCodeActive((DateTime)codeInfo.ExpiresAt!, codeInfo.isActive)
        /// </code>
        ///   after checks if the code hasnt expired and is still active, if expired it updates the code to inactive,
        ///   and generate a new code which is then stored in the EmailVerification table in the database
        ///   
        /// <para>
        /// If verification is successful, a unique login number is assigned to the user, and the code is marked as inactive.
        /// </para>
        /// 
        /// </remarks>
        /// <exception cref="UserNotFoundException"> Thrown if the user has been deleted from the database 
        /// (e.g., after 3 days).</exception>
        /// <exception cref="DailyAttemptsReachedException">Thrown if the user has reached the maximum of 
        ///  3 verification attempts for the day. </exception>
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

            var userInfo = await _userService.GetUserById(codeInfo.userId);

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

        /// <summary>
        /// Login user with their respective login details
        /// </summary>
        /// <param name="login">user login details</param>
        /// <remarks>
        /// <para>
        ///   Login process involves the following steps:
        /// </para>
        /// 
        /// <code>
        /// user = (await _userService.GetUserAsync(login))!;
        /// </code>
        /// <para>Verifies user login detils</para>
        /// </remarks>
        /// 
        /// <returns> Returns success if the user has successfully logged in with their respective 
        /// name and surname, otherwise, returns bad request
        /// </returns>
        /// <exception cref="UserNotFoundException">Thrown if the user is not found in the database</exception>"
        /// <exception cref="UserNotActivatedException">Thrown if the user has not verified their email</exception>"
        /// <exception cref="InvalidCredentialsException">Thrown if the user provides invalid login details</exception>""
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("SecureData")]
        public IActionResult GetSecureData()
        {

            _logger.LogInformation("SecureData endpoint called.");
            return Ok(new { HttpContext.User?.Identity?.Name });

        }

    }

}