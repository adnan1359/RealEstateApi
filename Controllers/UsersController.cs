using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstateApi.Data;
using RealEstateApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        ApiDbContext context = new ApiDbContext();
        private IConfiguration _config;


        public UsersController(IConfiguration config)
        {
            _config = config;
        }



        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            var userExists = context.Users.FirstOrDefault(u => u.Email == user.Email);


            if (userExists != null)
                return BadRequest("User with the same email already exists");


            context.Users.Add(user);
            context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }



        [HttpPost("[action]")]
        public IActionResult Login([FromBody] User user)
        {

            var currentUser = context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (currentUser != null)
                return NotFound();

            // Used to encrypt and decrypt the data
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));

            // To Hash our credential key
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Initialize the JWT Token Class
            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(jwt);
        }
    }
}
