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
        private readonly IUnitOfWork<ApplicationDbContext, ReaderDbContext> _unitOfWork;
        private readonly IProductBL productBL;

        public ProductsController(IUnitOfWork<ApplicationDbContext, ReaderDbContext> unitOfWork, IProductBL productBL)
        {
            _unitOfWork = unitOfWork;
            this.productBL = productBL;
        }

        [HttpGet("products")]
        [SwaggerOperation("Get Product List")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<List<ProductListResponseModel>>))]
        public async Task<IActionResult> GetProductList()
        {
            return Ok(await productBL.GetProductList());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductRequestModel createProductRequestModel)
        {
            return Ok(await productBL.CreateProduct(createProductRequestModel));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _unitOfWork.GetRepository<Product>().GetAsync(id, reader: true);

                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
