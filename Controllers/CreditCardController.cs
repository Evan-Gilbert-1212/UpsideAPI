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
    public DatabaseContext UpsideDb = new DatabaseContext();

    [HttpGet("{userId}")]
    public ActionResult GetUserCreditCards(int userId)
    {
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(UpsideDb.CreditCards.Where(acc => acc.UserID == userId)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> AddUserCreditCard(CreditCard newCreditCard, int userId)
    {
      newCreditCard.UserID = userId;

      UpsideDb.CreditCards.Add(newCreditCard);

      await UpsideDb.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newCreditCard),
        ContentType = "application/json",
        StatusCode = 201
      };
    }
  }
}