using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UpsideAPI.Models;
using UpsideAPI.ViewModels;

namespace UpsideAPI.Controllers
{
  [Route("auth")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    readonly private DatabaseContext _context;
    readonly private string JWT_KEY;

    public AuthController(DatabaseContext context, IConfiguration config)
    {
      JWT_KEY = config["JWT_KEY"];
      _context = context;
    }

    private string CreateJWT(User user)
    {
      var expirationTime = DateTime.UtcNow.AddHours(10);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
        {
          new Claim("ID", user.ID.ToString()),
          new Claim("FirstName", user.FirstName),
          new Claim("LastName", user.LastName),
          new Claim("UserName", user.UserName)
        }),
        Expires = expirationTime,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JWT_KEY)),
            SecurityAlgorithms.HmacSha256Signature
          )
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

      return token;
    }

    [HttpPost("signup")]
    public async Task<ActionResult> SignUpUser(IncomingUserData userData)
    {
      //User Name Validation
      if (userData.UserName == "")
      {
        return BadRequest("User Name cannot be blank");
      }

      //Password Validation
      if (userData.Password == "")
      {
        return BadRequest("Password cannot be blank");
      }

      //Hash Password
      var newUser = new User
      {
        FirstName = userData.FirstName,
        LastName = userData.LastName,
        UserName = userData.UserName,
      };

      var hashedPassword = new PasswordHasher<User>().HashPassword(newUser, userData.Password);

      newUser.HashedPassword = hashedPassword;

      //Save User
      _context.Users.Add(newUser);
      await _context.SaveChangesAsync();

      //Generate and return JWT Token  
      return Ok(new { Token = CreateJWT(newUser), UserInfo = newUser });
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginUser(IncomingLoginData loginData)
    {
      var userAccount = await _context.Users.FirstOrDefaultAsync(user => user.UserName == loginData.UserName);

      if (userAccount == null)
      {
        return BadRequest("User does not exist.");
      }

      var verifyResults = new PasswordHasher<User>().VerifyHashedPassword(userAccount, userAccount.HashedPassword, loginData.Password);

      if (verifyResults == PasswordVerificationResult.Success)
      {
        return Ok(new { Token = CreateJWT(userAccount), UserInfo = userAccount });
      }
      else
      {
        return BadRequest("Password does not match.");
      }
    }
  }
}