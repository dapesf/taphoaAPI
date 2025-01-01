
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class StoreController : ControllerBase
{
    private readonly TaphoaEntities context;

    public StoreController(TaphoaEntities _context)
    {
        context = _context;
    }

    [HttpGet]
    public IActionResult GetStore(string cd_store)
    {
        ma_store? ma_store = context.ma_store.Find(cd_store);
        return NotFound(new
        {
            status = "Get store sucessfull!",
            data = ma_store
        });
    }

    [HttpGet]
    public IActionResult GetAllStore()
    {
        List<ma_store> ma_stores = new List<ma_store>();
        ma_stores = context.ma_store.ToList();
        return Ok(new
        {
            status = "Take all store sucessfull!",
            data = ma_stores
        });
    }

    [HttpPost]
    public IActionResult DeleteStore(string cd_store)
    {
        try
        {
            ma_store? ma_store = context.ma_store.Find(cd_store);

            if (ma_store == null)
            {
                return NotFound(new
                {
                    status = "Not found store to delete"
                });
            }
            context.Remove(ma_store);
            context.SaveChanges();
            return Ok(new
            {
                status = "Delete Store sucessfull!",
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
    public IActionResult PostStore([FromBody] ma_store data)
    {
        try
        {
            context.Add(data);
            context.SaveChanges();
            return Ok(new
            {
                status = "Post Store sucessfull!",
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