using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kzy_entities.Entities;
using UnitOfWorkDemo1.BL;
using kzy_entities.Models.Request.Product;
using kzy_entities.Common;
using kzy_entities.Models.Response.Product;
using Swashbuckle.AspNetCore.Annotations; // Corrected Swagger annotation namespace
using Microsoft.AspNetCore.Authorization; // Use this for [Authorize]
using kzy_entities.Enums;

namespace UnitOfWorkDemo.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = ConstantStrings.ACCESSTOKENAUTH)]
    public class ProductsController : ControllerBase
    {
        private readonly IProductBL productBL;

        public ProductsController(IProductBL productBL)
        {
            this.productBL = productBL;
        }

        [HttpGet("get-product")]
        [SwaggerOperation("Get Product List")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<List<ProductListResponseModel>>))]
        public async Task<IActionResult> GetProductList()
        {
            return Ok(await productBL.GetProductList());
        }

        [HttpGet("{productId:guid}/product-detail")]
        [SwaggerOperation("Get Product Details")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<ProductListResponseModel>))]
        public async Task<IActionResult> GetProductDetails([FromRoute] Guid productId)
        {
            return Ok(await productBL.GetProductDetails(productId));
        }

        [HttpPost("create-product")]
        [SwaggerOperation("Create Product")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<string>))]
        public async Task<IActionResult> Create([Microsoft.AspNetCore.Mvc.FromBody] CreateProductRequestModel createProductRequestModel)
        {
            return Ok(await productBL.CreateProduct(createProductRequestModel));
        }

        [HttpPatch("{productId:guid}/update-product")]
        [SwaggerOperation("Update Product By Id")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<string>))]
        public async Task<IActionResult> UpdateProductById([FromRoute] Guid productId, [Microsoft.AspNetCore.Mvc.FromBody] UpdateProductRequestModel updateProductRequestModel)
        {
            return Ok(await productBL.UpdateProductById(productId, updateProductRequestModel));
        }

        [HttpDelete("{productId:guid}")]
        [SwaggerOperation("Delete Product")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<string>))]
        public async Task<IActionResult> Delete([FromRoute] Guid productId)
        {
            return Ok(await productBL.DeleteProduct(productId));
        }
    }
}
