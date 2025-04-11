
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InterFace;
using Microsoft.OData.ModelBuilder;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly AppDBContext context;

    public ProductController(AppDBContext _context)
    {
        context = _context;
    }

    [HttpGet]
    public IActionResult GetProduct(string cd_product, string cd_store)
    {
        tr_product? tr_product = context.tr_product.Find([cd_product, cd_store]);
        return Ok(new ResponseResult
        (
            "I200",
            "Get Product sucessfull!",
            tr_product
        ));
    }

    [HttpGet]
    public IActionResult GetAllProduct()
    {
        var products = from product in context.tr_product
                       join unit in (context.ma_literal.Where(x => x.cd_type == "001"))
                       on product.type_unit equals unit.kbn1
                       into dataGroup
                       from data in dataGroup.DefaultIfEmpty()
                       orderby product.cd_product descending
                       select new ProductResponse
                       {
                           cd_product = product.cd_product
                           ,
                           cd_store = product.cd_store
                           ,
                           nm_product = product.nm_product
                           ,
                           nm_product_en = product.nm_product_en
                           ,
                           dt_start = product.dt_start
                           ,
                           dt_end = product.dt_end
                           ,
                           kin_price = product.kin_price
                           ,
                           type_unit = product.type_unit
                           ,
                           qnt_in = product.qnt_in
                           ,
                           qnt_remain = product.qnt_remain
                           ,
                           cd_country = product.cd_country
                           ,
                           unit = data.nm1
                       };

        return Ok(new ResponseResult
        (
            "I200",
            "Get Products sucessfull!",
            products
        ));
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

public class ProductResponse
{
    public string cd_product { get; set; }
    public string cd_store { get; set; }
    public string nm_product { get; set; }
    public string nm_product_en { get; set; }
    public DateOnly dt_start { get; set; }
    public DateOnly dt_end { get; set; }
    public double kin_price { get; set; }
    public string type_unit { get; set; }
    public int qnt_in { get; set; }
    public int qnt_remain { get; set; }
    public string cd_country { get; set; }
    public string unit { get; set; }
}
