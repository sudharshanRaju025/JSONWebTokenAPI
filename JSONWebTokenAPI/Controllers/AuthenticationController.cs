using JSONWebTokenAPI.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JSONWebTokenAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly Authentication.ILogger<AuthenticationController> logger;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly TokenService _token;
        private readonly RefreshTokenService refreshTokenService;
        private readonly AppDbContext _context;
        private readonly ICredentialsValidator validator;
      
        public AuthenticationController(
                UserManager<ApplicationUser> _usermanager,
                RoleManager<IdentityRole> _rolemanager, 
                TokenService Token,
                AppDbContext context,
                RefreshTokenService service,
                Authentication.ILogger<AuthenticationController> _logger,
                ICredentialsValidator _validator
            )
        {
            userManager = _usermanager;
            roleManager = _rolemanager;
            _token = Token;
            _context=context;
            refreshTokenService = service;
            logger = _logger;
            validator = _validator;
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] Login model)
        {
            logger.LogInformation("login started");

            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password)) 
            {
                return BadRequest(new Response { Status = "404", Message = "Credentials not found" });
            }
            var roles = await userManager.GetRolesAsync(user);

            var accessToken = _token.GenerateJwtToken(user, roles);
            var refreshToken = refreshTokenService.CreateRefreshToken(Guid.NewGuid().ToString(), user.Id);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            logger.LogInformation("refresh token is successfully stored in the database");

            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(10)
            };
            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(20)
            };

            Response.Cookies.Append("accessToken", accessToken, accessTokenOptions);
            Response.Cookies.Append("refreshToken", refreshToken.Token, refreshTokenOptions);

            logger.LogInformation("successfully created the Access and refresh tokens");

            
            return Ok(new Response { Status="200",Message="Successfully created tokens"});
        }

        
        [HttpPost]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (request.AccessToken == null || request.RefreshToken == null)
                return BadRequest("Invalid client request");

            logger.LogInformation("Checking the refresh token with database");
             var stored = await _context.RefreshTokens
                 .IgnoreQueryFilters()
                              .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);
            if (stored == null )
            {
                return Unauthorized("Invalid or expire token");
            }
            
            stored.Invalidated = true;
            await _context.SaveChangesAsync();

            var user = await userManager.FindByIdAsync(stored.UserId);
            if (user == null)
                return Unauthorized("User not found");

            var roles = await userManager.GetRolesAsync(user);
            logger.LogInformation("generating the new access and Refresh tokens");

            var newAccess = _token.GenerateJwtToken(user, roles);
            var newRefresh = refreshTokenService.CreateRefreshToken(stored.JwtId, stored.UserId);
            _context.RefreshTokens.Add(newRefresh);
            await _context.SaveChangesAsync();

            logger.LogInformation("successfully created new access and refresh tokens");

            return Ok(new RefreshTokenResponse
            {
                AccessToken = newAccess,
                RefreshToken = newRefresh.Token
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            logger.LogInformation("registering the User role ");
            var Existuser = await userManager.FindByNameAsync(model.Username);
            if (Existuser != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            }

            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
               
            };

            if (!validator.IsValidUsername(model.Username) )
            {
                return BadRequest(new Response { Status = "Error", Message = "Invalid username or password or Email format" });
            }

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Creation is failed.please check the entered details!!." });

            await userManager.AddToRoleAsync(user, UserRoles.User);
            logger.LogInformation("created the new User");

            return Ok();
        }

        [HttpPost]
       
        public async Task<IActionResult> RegisterAdmin([FromBody] Register model)
        {
           
           
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
               
            };

            if (!validator.IsValidUsername(model.Username))
                return BadRequest(new Response { Status = "Error", Message = "Invalid Username format" });

            if (!validator.IsValidPassword(model.Password))
                return BadRequest(new Response { Status = "Error", Message = "Invalid Password format" });

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status406NotAcceptable, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

    }
}