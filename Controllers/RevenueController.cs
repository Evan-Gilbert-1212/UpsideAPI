using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpsideAPI.Models;
using Newtonsoft.Json;
using System;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class RevenueController : ControllerBase
  {
    private DatabaseContext _context;

    public RevenueController(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet("{userId}")]
    public ActionResult GetUserRevenues(int userId, DateTime BeginDate, DateTime EndDate)
    {
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.Revenues
                                                .Where(rev =>
                                                  rev.UserID == userId
                                                  && rev.RevenueDate >= BeginDate
                                                  && rev.RevenueDate <= EndDate)
                                                .OrderBy(rev => rev.RevenueDate)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> AddUserRevenue(Revenue newRevenue, int userId)
    {
      newRevenue.UserID = userId;

      _context.Revenues.Add(newRevenue);

      await _context.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newRevenue),
        ContentType = "application/json",
        StatusCode = 201
      };
    }
  }
}