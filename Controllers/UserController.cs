using Microsoft.AspNetCore.Mvc;
using MovieShop.Service;
using MovieShopAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using MovieShop.Core.Entities;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MovieShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        public UserController(IUserService userService, IConfiguration config)
        {
            _config=config;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDTO user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            var createdUser = await _userService.CreateUser(user.UserName, user.Password, user.FirstName, user.LastName);

            if (createdUser == null)
            {
                return BadRequest("User ame Already Exist.");

            }
            return Ok("user Successfully Created!");
        }

        [HttpPost("login")]

        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDTO userLogin)
        {
            if (userLogin == null)
            {
                return BadRequest();

            }
            var user = await _userService.ValidateUser(userLogin.UserName, userLogin.Password);
            if (user == null)
            {
                return NotFound("User name does not exist");
            }
            return Ok(new { Token = GenerateToken(user, new List<UserRole>()) });
        }
            //Generate Token only if username and password is valid 
        private string GenerateToken(User user, IEnumerable<UserRole> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                //new Claim(JwtRegisteredClaimNames.Birthdate, user.DateOfBirth?.ToShortDateString()),
                //new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                //new Claim("alias", user.FirstName[0] + user.LastName[0].ToString()),
                //new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName)
            };
                // var roles =  _userService.GetUserRoles(user.Email).GetAwaiter().GetResult();
            foreach (var role in roles)
                    // make sure you have role with small "r" as Authorization looks for role with comma separated array
                claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenSettings:PrivateKey"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
                var expires = DateTime.Now.AddDays(Convert.ToDouble(_config["TokenSettings:ExpirationDays"]));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expires,
                    SigningCredentials = credentials,
                    Issuer = _config["TokenSettings:Issuer"],
                    Audience = _config["TokenSettings:Audience"]
                };
                var encodedJwt = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
                return new JwtSecurityTokenHandler().WriteToken(encodedJwt);
        }

            //return Ok("Successfully Login");

    }
}
