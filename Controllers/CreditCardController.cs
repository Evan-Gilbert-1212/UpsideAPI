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
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

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
      _context.Entry(cardToUpdate).State = EntityState.Modified;
      await _context.SaveChangesAsync();

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
      if (newCreditCard.CardIssuer == "")
      {
        return BadRequest("Credit Card Issuer cannot be blank.");
      }

      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      newCreditCard.UserID = userId;

      _context.CreditCards.Add(newCreditCard);

      await _context.SaveChangesAsync();

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
      var creditCardToDelete = await _context.CreditCards.FirstOrDefaultAsync(card => card.ID == creditCardId);
      _context.CreditCards.Remove(creditCardToDelete);
      await _context.SaveChangesAsync();

      return Ok();
    }
  }
}