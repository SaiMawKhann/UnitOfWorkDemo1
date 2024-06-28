using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using kzy_entities.Entities;
using UnitOfWorkDemo1.BL;
using UnitOfWorkDemo.Repositories;
using kzy_entities.Models.Request.Product;
using kzy_entities.DBContext;
using Swashbuckle.Swagger.Annotations;
using kzy_entities.Common;
using kzy_entities.Models.Response.Product;

namespace UnitOfWorkDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductBL productBL;

        public ProductsController(IProductBL productBL)
        {
            this.productBL = productBL;
        }

        [HttpGet("Products")]
        [SwaggerOperation("Get Product List")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<List<ProductListResponseModel>>))]
        public async Task<IActionResult> GetProductList()
        {
            return Ok(await productBL.GetProductList());
        }
        [HttpGet("Product Details")]
        [SwaggerOperation("Get Product Details")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<List<ProductListResponseModel>>))]
        public async Task<IActionResult> GetProductDetails([FromRoute]Guid id)
        {
            return Ok(await productBL.GetProductDetails(id));
        }

        [HttpPost]
        [SwaggerOperation("Create Product")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<string>))]
        public async Task<IActionResult> Create([FromBody]CreateProductRequestModel createProductRequestModel)
        {
            return Ok(await productBL.CreateProduct(createProductRequestModel));
        }

        [HttpPatch]
        [SwaggerOperation("Get Product By Id")]
        public async Task<IActionResult> UpdateProductById([FromRoute]Guid id, [FromBody] UpdateProductRequestModel updateProductRequestModel)
        {
            return Ok(await productBL.UpdateProductById(id, updateProductRequestModel));
         }

        [HttpDelete]
        [SwaggerOperation("Delete Product")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<string>))]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
             return Ok(await productBL.DeleteProduct(id));
        }
    }
}
