using JSONWebTokenAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace JSONWebTokenAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
   
    public class WeatherForecastController : ControllerBase
    {
        List<User> _user = new List<User>
         {
          new User { UserId = 1, Name = "Alice Smith", Email = "alice@gmail.com",PhoneNo="9392486441", Address = "123 Elm Street, Nagpur" },
          new User { UserId = 2, Name = "Alex", Email = "alex@gmail.com",PhoneNo="9392486456", Address = "hs-890, loni" },
        };

        private readonly Authentication.ILogger<WeatherForecastController> _logger;
        public WeatherForecastController(Authentication.ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Public")]
        public ActionResult GetCount()
        {
            var result = _user.Count();
            return Ok(result);
        }

        [HttpGet("User")]
        [Authorize(Roles = "Admin,User")]
        public new ActionResult<IEnumerable<User>> User()
        {
            var result = _user.
                Select(u => new { u.UserId, u.Name, }).
                ToList();
            return Ok(result);
        }

        [HttpGet("Admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<User>> Admin()
        {
            var projected = _user
                .Select(u => new { u.UserId, u.Name, u.Email, u.PhoneNo, u.Address })
                .ToList();

            return Ok(projected);
        }

    }
}
