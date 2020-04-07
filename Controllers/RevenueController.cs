using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpsideAPI.Models;
using Newtonsoft.Json;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class RevenueController : ControllerBase
  {
    public DatabaseContext UpsideDb = new DatabaseContext();

    [HttpGet("{userId}")]
    public ActionResult GetUserRevenues(int userId)
    {
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(UpsideDb.Revenues.Where(exp => exp.UserID == userId)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> AddUserRevenue(Revenue newRevenue, int userId)
    {
      newRevenue.UserID = userId;

      UpsideDb.Revenues.Add(newRevenue);

      await UpsideDb.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newRevenue),
        ContentType = "application/json",
        StatusCode = 201
      };
    }
  }
}