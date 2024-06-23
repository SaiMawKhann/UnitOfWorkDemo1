using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UnitOfWorkDemo.Interfaces;
using UnitOfWorkDemo.Models;

namespace UnitOfWorkDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync(true);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id, reader: true);

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
