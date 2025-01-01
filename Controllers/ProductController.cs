
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly TaphoaEntities context;

    public ProductController(TaphoaEntities _context)
    {
        context = _context;
    }

    [HttpGet]
    public IActionResult GetProduct(string cd_product, string cd_store)
    {
        tr_product? tr_anounce = context.tr_product.Find([cd_product, cd_store]);
        return NotFound(new
        {
            status = "Get Product sucessfull!",
            data = tr_anounce
        });
    }

    [HttpGet]
    public IActionResult GetAllProduct()
    {
        List<tr_product> tr_products = new List<tr_product>();
        tr_products = context.tr_product.ToList();
        return Ok(new
        {
            status = "Post Product sucessfull!",
            data = tr_products
        });
    }

    [HttpPost]
    public IActionResult DeleteProduct(string cd_product, string cd_store)
    {
        try
        {
            tr_product? tr_product = context.tr_product.Find([cd_product, cd_store]);

            if (tr_product == null)
            {
                return NotFound(new
                {
                    status = "Not found product to delete"
                });
            }
            context.Remove(tr_product);
            context.SaveChanges();
            return Ok(new
            {
                status = "Delete product sucessfull!",
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
    public IActionResult PostProduct([FromBody] tr_product data)
    {
        try
        {
            context.Add(data);
            context.SaveChanges();
            return Ok(new
            {
                status = "Post Product sucessfull!",
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