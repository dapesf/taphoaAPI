
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InterFace;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class LiteralController : ControllerBase
{
    private readonly TaphoaEntities context;

    public LiteralController(TaphoaEntities _context)
    {
        context = _context;
    }

    [HttpGet]
    public IActionResult GetLiteral(string cd_type, string kbn1)
    {
        ma_literal literal = context.ma_literal.Find([cd_type, kbn1]);
        return Ok(new ResponseResult("I200", "Lấy data literal thành công.", literal));
    }

    [HttpGet]
    public IActionResult GetAllLiteral()
    {
        List<ma_literal> ma_literals = new List<ma_literal>();
        ma_literals = context.ma_literal.ToList();
        return Ok(new
        {
            status = "Take all literal sucessfull!",
            data = ma_literals
        });
    }

    [HttpPost]
    public IActionResult DeleteLiteral(string cd_type, string kbn1)
    {
        try
        {
            ma_literal? ma_literal = context.ma_literal.Find([cd_type, kbn1]);

            if (ma_literal == null)
            {
                return NotFound(new
                {
                    status = "Not found literal to delete"
                });
            }
            context.Remove(ma_literal);
            context.SaveChanges();
            return Ok(new
            {
                status = "Delete literal sucessfull!",
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
    public IActionResult PostLiteral([FromBody] ma_literal data)
    {
        try
        {
            context.Add(data);
            context.SaveChanges();
            return Ok(new
            {
                status = "Post literal sucessfull!",
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