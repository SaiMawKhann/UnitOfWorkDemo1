using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UnitOfWorkDemo.Interfaces;
using UnitOfWorkDemo.Models;
using UnitOfWorkDemo.Services;

//[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDataSynchronizationService _dataSynchronizationService;

    public ProductsController(IUnitOfWork unitOfWork, IDataSynchronizationService dataSynchronizationService)
    {
        _unitOfWork = unitOfWork;
        _dataSynchronizationService = dataSynchronizationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products = await _unitOfWork.ReaderProducts.GetAllAsync();
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

            await _dataSynchronizationService.SynchronizeProductsAsync();

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
            var product = await _unitOfWork.GetReaderRepository<Product>()
                                            .Query(p => p.Id == id)
                                            .FirstOrDefaultAsync();

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
