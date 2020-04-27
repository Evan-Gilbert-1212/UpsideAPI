using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpsideAPI.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

  public class CreditCardController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public CreditCardController(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet]
    public ActionResult GetUserCreditCards()
    {
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Return all Credit Cards for associated User ID
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.CreditCards
                                              .Where(card => card.UserID == userId)
                                              .OrderBy(card => card.CardIssuer)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUserCreditCard(CreditCard cardToUpdate)
    {
      //If Card Issuer is blank, return BadRequest
      if (cardToUpdate.CardIssuer == "")
      {
        return BadRequest("Credit Card Issuer cannot be blank.");
      }

      //Else, set state of incoming entry to "Modified"
      _context.Entry(cardToUpdate).State = EntityState.Modified;

      //Save Changes
      await _context.SaveChangesAsync();

      //Return updated Credit Card
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(cardToUpdate),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost]
    public async Task<ActionResult> AddUserCreditCard(CreditCard newCreditCard)
    {
      //If Card Issuer is blank, return BadRequest
      if (newCreditCard.CardIssuer == "")
      {
        return BadRequest("Credit Card Issuer cannot be blank.");
      }

      //Else, get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Add User ID to new Credit Card
      newCreditCard.UserID = userId;

      //Add Credit Card to table
      _context.CreditCards.Add(newCreditCard);

      //Save changes
      await _context.SaveChangesAsync();

      //Return new Credit Card
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newCreditCard),
        ContentType = "application/json",
        StatusCode = 201
      };
    }

    [HttpDelete("{creditCardId}")]
    public async Task<ActionResult> DeleteUserCreditCard(int creditCardId)
    {
      //Locate Credit Card to delete by ID
      var creditCardToDelete = await _context.CreditCards.FirstOrDefaultAsync(card => card.ID == creditCardId);

      //Remove Credit Card from table
      _context.CreditCards.Remove(creditCardToDelete);

      //Save changes
      await _context.SaveChangesAsync();

      //Return OK
      return Ok();
    }
  }
}