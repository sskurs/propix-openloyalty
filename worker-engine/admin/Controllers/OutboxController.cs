using Microsoft.AspNetCore.Mvc;
using Admin.Data;
using Admin.Models;
[ApiController]
[Route("api/[controller]")]
public class OutboxController : ControllerBase {
  private readonly AdminDbContext _db;
  public OutboxController(AdminDbContext db){_db=db;}
  [HttpPost("enqueue")]
  public IActionResult Enqueue([FromBody] OutboxMessage m){
    _db.Outbox.Add(m); _db.SaveChanges();
    return Ok(m);
  }
}

