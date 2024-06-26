using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UnitOfWorkDemo.Interfaces;
using kzy_entities.Entities;
using UnitOfWorkDemo1.BL;
using UnitOfWorkDemo.Repositories;
using kzy_entities.Models.Request.Product;
using kzy_entities.DBContext;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _unitOfWork.GetRepository<Product>().GetAllAsync(true);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductRequestModel createProductRequestModel)
        {
            return Ok(productBL.CreateProduct(createProductRequestModel));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(id, reader: true);

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
