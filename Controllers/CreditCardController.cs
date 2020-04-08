using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpsideAPI.Models;
using Newtonsoft.Json;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class CreditCardController : ControllerBase
  {
    private DatabaseContext _context;

    public CreditCardController(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet("{userId}")]
    public ActionResult GetUserCreditCards(int userId)
    {
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.CreditCards.Where(acc => acc.UserID == userId)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> AddUserCreditCard(CreditCard newCreditCard, int userId)
    {
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
  }
}