using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ODataService;

[Route("api/[controller]")]
[Authorize]
public class ODataServiceController : ODataController
{
    private readonly AppDBContext context;

    public ODataServiceController(AppDBContext _context)
    {
        context = _context;
    }

    [EnableQuery]
    [HttpGet("ma_literal")]
    public IQueryable<ma_literal> Get()
    {
        return context.ma_literal.AsQueryable();
    }
}
