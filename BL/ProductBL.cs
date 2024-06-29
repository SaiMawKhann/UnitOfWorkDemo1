using kzy_entities.Common;
using kzy_entities.DBContext;
using kzy_entities.Entities;
using kzy_entities.Models.Request.Product;
using UnitOfWorkDemo.Repositories;
using System.Threading.Tasks;
using System;
using System.Threading;
using kzy_entities.Models.Response.Product;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using kzy_entities.Constants;

namespace UnitOfWorkDemo1.BL
{
    #region interfaces
    public interface IProductBL
    {
        Task<ResponseModel<string>> CreateProduct(CreateProductRequestModel createRequestModel);
        Task<ResponseModel<List<ProductListResponseModel>>> GetProductList();
        Task<ResponseModel<ProductDetailResponseModel>> GetProductDetails(Guid id);
        Task<ResponseModel<string>> UpdateProductById(Guid id, UpdateProductRequestModel model);
        Task<ResponseModel<string>> DeleteProduct(Guid id);

    }
    #endregion

    public class ProductBL :  IProductBL
    {
        private readonly IUnitOfWork<ApplicationDbContext, ReaderDbContext> _unitOfWork;
        private readonly IErrorCodeProvider _errorCodeProvider;
        protected readonly IMapper _mapper;


        public ProductBL(IUnitOfWork<ApplicationDbContext, ReaderDbContext> unitOfWork,
                                     IErrorCodeProvider errorCodeProvider,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _errorCodeProvider = errorCodeProvider;
            _mapper = mapper;
        }
        public async Task<ResponseModel<List<ProductListResponseModel>>> GetProductList()
        {
            try
            {
                var products = await _unitOfWork.GetRepository<Product>()
                    .Query(x => x.Price > 0, true)
                    .OrderByDescending(x => x.CreatedOn)
                    .ToListAsync();

                var data = _mapper.Map<List<ProductListResponseModel>>(products);

                return await _errorCodeProvider.GetResponseModelAsync<List<ProductListResponseModel>>(ErrorCode.E0, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return await _errorCodeProvider.GetResponseModelAsync<List<ProductListResponseModel>>(ErrorCode.E500);
            }
        }
        public async Task<ResponseModel<ProductDetailResponseModel>> GetProductDetails(Guid id)
        {
            try
            {
                var product = await _unitOfWork.GetRepository<Product>()
                    .Query(x => x.Id == id, true).FirstOrDefaultAsync();

                if(product == null)
                {
                    return await _errorCodeProvider.GetResponseModelAsync<ProductDetailResponseModel>(ErrorCode.E404, null);
                }

                var data = _mapper.Map<ProductDetailResponseModel>(product);

                return await _errorCodeProvider.GetResponseModelAsync<ProductDetailResponseModel> (ErrorCode.E0, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return await _errorCodeProvider.GetResponseModelAsync<ProductDetailResponseModel>(ErrorCode.E500);
            }
        }

        public async Task<ResponseModel<string>> CreateProduct(CreateProductRequestModel model)
        {
            try
            {
                Product product = new Product
                {
                    Id = new Guid(),
                    Name = model.Name,
                    Price = model.Price,
                    CreatedBy = null,
                    CreatedOn = DateTime.UtcNow,
                };

                await _unitOfWork.GetRepository<Product>().AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                return await _errorCodeProvider.GetResponseModelAsync<string>(ErrorCode.E0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return await _errorCodeProvider.GetResponseModelAsync<string>(ErrorCode.E500);
            }
        }
        public async Task<ResponseModel<string>> UpdateProductById(Guid id, UpdateProductRequestModel model)
        {
                try
                {
                    var product = await _unitOfWork.GetRepository<Product>().Query(x=> x.Id == model.Id).FirstOrDefaultAsync();

                    if (product == null)
                    {
                        return await _errorCodeProvider.GetResponseModelAsync<string>(ErrorCode.E404, "Product not found");
                    }

                    product.Name = model.Name;
                    product.Price = model.Price;
                    product.UpdatedOn = DateTime.UtcNow;

                    _unitOfWork.GetRepository<Product>().Update(product);
                    await _unitOfWork.SaveChangesAsync();

                    return await _errorCodeProvider.GetResponseModelAsync<string>(ErrorCode.E0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return await _errorCodeProvider.GetResponseModelAsync<string>(ErrorCode.E500);
                }
        }

        public async Task<ResponseModel<string>> DeleteProduct(Guid id)
        {
            try
            {
                var product =await _unitOfWork.GetRepository<Product>().Query(x=> x.Id == id).FirstOrDefaultAsync();

                if (product == null)
                {
                    return await _errorCodeProvider.GetResponseModelAsync<string>(ErrorCode.E404, "Product not found");
                }
                 await _unitOfWork.GetRepository<Product>().DeleteAsync(product);
                await _unitOfWork.SaveChangesAsync();
                return await _errorCodeProvider.GetResponseModelAsync<string>(ErrorCode.E0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return await _errorCodeProvider.GetResponseModelAsync<string>(ErrorCode.E500);
            }
        }
    }
}
