
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class AnnounceController : ControllerBase
{
    private readonly AppDBContext context;

    public AnnounceController(AppDBContext _context)
    {
        context = _context;
    }

    [HttpGet]
    public IActionResult GetAnnounces(string id)
    {
        tr_anounce? tr_anounce = context.tr_anounce.Find(id);
        return NotFound(new
        {
            status = "Get Announces sucessfull!",
            data = tr_anounce
        });
    }

    [HttpGet]
    public List<tr_anounce> GetAllAnnounces()
    {
        List<tr_anounce> tr_anounces = new List<tr_anounce>();
        tr_anounces = context.tr_anounce.ToList();
        return tr_anounces;
    }

    [HttpPost]
    public IActionResult DeleteAnnounces(string id)
    {
        try
        {
            tr_anounce? tr_anounce = context.tr_anounce.Find(id);

            if (tr_anounce == null)
            {
                return NotFound(new
                {
                    status = "Not found anounce to delete"
                });
            }
            context.Remove(tr_anounce);
            context.SaveChanges();
            return Ok(new
            {
                status = "Post Announces sucessfull!",
            });
        }
        catch (Exception ex)
        {
            return NotFound(new
            {
                status = ex.Message,
            });
        }
    }

    [HttpPost]
    public IActionResult PostAnnounces([FromBody] tr_anounce data)
    {
        try
        {
            context.Add(data);
            context.SaveChanges();
            return Ok(new
            {
                status = "Post Announces sucessfull!",
            });
        }
        catch (Exception ex)
        {
            return NotFound(new
            {
                status = ex.Message,
            });
        }
    }
}