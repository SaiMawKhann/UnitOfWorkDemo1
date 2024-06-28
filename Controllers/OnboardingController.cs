using kzy_entities.Common;
using kzy_entities.DBContext;
using kzy_entities.Entities;
using kzy_entities.Models.Request.Product;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Swagger.Annotations;
using UnitOfWorkDemo.Repositories;
using UnitOfWorkDemo1.BL;

namespace UnitOfWorkDemo1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnboardingController : Controller
    {
        //public OnboardingController()
        //{

        //}
        //[HttpPost]
        //[SwaggerOperation("Create Product")]
        //[SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<string>))]
        //public async Task<IActionResult> LogIn([FromBody] CreateProductRequestModel createProductRequestModel)
        //{
        //    return Ok(await productBL.CreateProduct(createProductRequestModel));
        //}
    }
}
