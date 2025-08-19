using Microsoft.AspNetCore.Mvc;
using JSONWebTokenAPI.Authentication;
using JSONWebTokenAPI.Model;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
namespace JSONWebTokenAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]/[Action]")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Authentication.ILogger<UserController> _logger;

        public UserController(
            AppDbContext context, 
            Authentication.ILogger<UserController> logger
            )
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var result = await _context.users.ToListAsync();
            _logger.LogInformation("getting the details");
            return Ok(result);
        }

        [HttpGet("{id}")]
       
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var result = await _context.users.FindAsync(id);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "404", Message = "No Content" });
            }
            if (result != null)
            {
                return result;
            }
           return Unauthorized();
        }

        [HttpPost]
        //[Authorize(Roles ="Admin")]
        public async Task<ActionResult<User>> Create([FromBody] User model)
        {
            if (model.Name == null)
            { 
                return NoContent();
            }
             _context.users.Add(model);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User Created successfully.");
            return CreatedAtAction(nameof(GetUser), new { id = model.UserId }, model);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] User todoItem)
        {
            var existingItem = await _context.users.FindAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }
            if(existingItem != null)
            {
                existingItem.Name = todoItem.Name;
                existingItem.Address = todoItem.Address;
                existingItem.PhoneNo = todoItem.PhoneNo;
                existingItem.Email = todoItem.Email;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User id:{id} updated successfully");
                return Ok(new Response { Status = "200",Message="User Updated Successfully." });
            }
            return Unauthorized();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles ="Admin")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var check = await _context.users.FindAsync(id);
            if (check == null)
            {
                _logger.LogError($"User id :{id} is not found");
                return NotFound();
            }
            if (check != null)
            {
                _context.users.Remove(check);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User Id:{id} Deleted successfully");
                return Ok(new Response { Status="200",Message="User Deleted Successfully."});
            }
            _logger.LogWarning("Unauthorized");
            return Unauthorized(StatusCodes.Status401Unauthorized);
        }

        [HttpGet]
        //[Authorize(Roles="User,Admin")]
        public async Task<ActionResult<IEnumerable<User>>> UserEndPoint()
        {
            var projected = await _context.users
                .AsNoTracking()
                .Select(u=>new { u.Name, u.UserId }).ToListAsync();
            _logger.LogInformation("fetched the details related to the User role");
            return Ok(projected);
        }

        [HttpGet]
        //[Authorize(Roles="Admin")]
        public async Task<ActionResult<IEnumerable<User>>> AdminEndPoint()
        {
            var fetch = await _context.users.AsNoTracking()
                .Select(u => new { u.UserId, u.Name, u.Email, u.PhoneNo,u.Address, })
                .ToListAsync();
            _logger.LogInformation("fetched the Admin role access details");
            return Ok(fetch);
        }
    }
}
