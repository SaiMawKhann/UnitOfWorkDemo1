using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UnitOfWorkDemo.Interfaces;
using UnitOfWorkDemo.Models;

namespace UnitOfWorkDemo.Services
{
    public class DataSynchronizationService : IDataSynchronizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitOfWork _readerUnitOfWork;

        public DataSynchronizationService(IUnitOfWork unitOfWork, IUnitOfWork readerUnitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _readerUnitOfWork = readerUnitOfWork ?? throw new ArgumentNullException(nameof(readerUnitOfWork));
        }

        public async Task SynchronizeProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();

                foreach (var product in products)
                {
                    var existingProduct = await _readerUnitOfWork.GetRepository<Product>()
                                                                  .Query(p => p.Id == product.Id)
                                                                  .AsNoTracking()
                                                                  .FirstOrDefaultAsync();

                    if (existingProduct == null)
                    {
                        await _readerUnitOfWork.ReaderProducts.AddAsync(new Product
                        {
                            Id = product.Id, 
                            Name = product.Name,
                            Price = product.Price
                        });
                    }
                    else
                    {
                        existingProduct.Name = product.Name;
                        existingProduct.Price = product.Price;
                        _readerUnitOfWork.ReaderProducts.Update(existingProduct);
                    }
                }
                await _readerUnitOfWork.CompleteAsyncForReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error synchronizing products: {ex.Message}");
                throw; 
            }
        }
    }
}
