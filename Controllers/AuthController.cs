using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
    private readonly DatabaseContext _context;
    private readonly string JWT_KEY;

    public AuthController(DatabaseContext context, IConfiguration config)
    {
      JWT_KEY = config["JWT_KEY"];
      _context = context;
    }

    private string CreateJWT(User user)
    {
      //Give all tokens 10 hour expiration time
      var expirationTime = DateTime.UtcNow.AddHours(10);

      //Define parameters of the token: Claims, Expiration time and Signing Credentials
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

      //Create token
      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

      //Return token
      return token;
    }

    public class TokenVerifier
    {
      public string tokenToValidate { get; set; }
    }

    [HttpPost("verifytoken")]
    public bool VerifyToken(TokenVerifier tokenVerifier)
    {
      //Define parameters to which the token will be checked against
      var parameters = new TokenValidationParameters
      {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JWT_KEY))
      };

      //Create new token handler and JwtSecurityToken
      var handler = new JwtSecurityTokenHandler();

      SecurityToken validatedToken = new JwtSecurityToken();

      //Validate token, return false if token is invalid or expired
      try
      {
        handler.ValidateToken(tokenVerifier.tokenToValidate, parameters, out validatedToken);
      }
      catch (SecurityTokenException)
      {
        return false;
      }
      catch (System.ArgumentException)
      {
        return false;
      }

      //Otherwise return true as token is valid
      return validatedToken != null;
    }

    [HttpPost("signup")]
    public async Task<ActionResult> SignUpUser(IncomingUserData userData)
    {
      //Check to see if the User Name already exists   
      var userExists = _context.Users.Any(user => user.UserName == userData.UserName);

      //If User Already Exists, return BadRequest
      if (userExists)
      {
        return BadRequest("User Name already in use");
      }

      //If First Name is blank, return BadRequest (Home page uses this property)
      if (userData.FirstName == "")
      {
        return BadRequest("First Name cannot be blank");
      }

      //If User Name is blank, return BadRequest
      if (userData.UserName == "")
      {
        return BadRequest("User Name cannot be blank");
      }

      //If Password is blank, return BadRequest
      if (userData.Password == "")
      {
        return BadRequest("Password cannot be blank");
      }

      //Otherwise, proceed to create new User
      var newUser = new User
      {
        FirstName = userData.FirstName,
        LastName = userData.LastName,
        UserName = userData.UserName,
        IsDemoAccount = false,
      };

      //Hash new users password
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
      //Locate Account
      var userAccount = await _context.Users.FirstOrDefaultAsync(user => user.UserName == loginData.UserName);

      //If Account not located, return BadRequest
      if (userAccount == null)
      {
        return BadRequest("Login Unsuccessful. Please try again.");
      }

      //Verify Hashed Password
      var verifyResults = new PasswordHasher<User>().VerifyHashedPassword(userAccount, userAccount.HashedPassword, loginData.Password);

      //If verified, log in. Else, return BadRequest
      if (verifyResults == PasswordVerificationResult.Success)
      {
        return Ok(new { Token = CreateJWT(userAccount), UserInfo = userAccount });
      }
      else
      {
        return BadRequest("Login Unsuccessful. Please try again.");
      }
    }

    [HttpPost("demouser")]
    public async Task<ActionResult> CreateDemoUser()
    {
      //Create new demo account for user
      var newUser = new User
      {
        FirstName = "Guest",
        LastName = "User",
        DisplayPeriod = "Wages",
        IsDemoAccount = true,
      };

      //Increment User Name so that all demo accounts have a unique User Name
      var demoUserID = 1;

      while (_context.Users.Any(user => user.UserName == "demo-user-" + demoUserID.ToString()))
      {
        demoUserID++;
      }

      //Assign unique User Name
      newUser.UserName = "demo-user-" + demoUserID.ToString();

      //Hash standard password, could be updated to be a config var
      newUser.HashedPassword = new PasswordHasher<User>().HashPassword(newUser, "Welcome");

      //Save Demo User
      _context.Users.Add(newUser);
      await _context.SaveChangesAsync();

      //Generate demo data for demo user
      DemoDataManager.CreateDemoData(newUser.ID);

      //Generate and return JWT Token  
      return Ok(new { Token = CreateJWT(newUser), UserInfo = newUser });
    }
  }
}